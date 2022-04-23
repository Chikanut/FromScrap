using System.Collections.Generic;
using System.Linq;
using Cars.View.Components;
using Kits.Components;
using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.Upgrades;
using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace UI.Upgrades
{
	public class UpgradeScreenMediator : Mediator<UpgradeScreenView>
	{
		public class PlatformInfo
		{
			public int ID;
			public List<KitType> Connections = new List<KitType>();
			public bool isFree;
			public List<KitComponent> ConnectedKits = new List<KitComponent>();
			public List<KitIDComponent> ConnectedKitsIDs = new List<KitIDComponent>();
		}

		private EntityManager _entityManager;
		private ISignalService _signalService;
		private ICarsConfigController _carsConfigController;

		public class UpgradesInfoDataBuffer
		{
			public CarConfigData carData;
			public Entity carEntity;
			public int carID;
			public int carLevel;
			public List<PlatformInfo> platformInfos = new List<PlatformInfo>();
		}

		private UpgradesInfoDataBuffer _data;
		

		[Inject]
		public void Init(ISignalService signalService, ICarsConfigController carsConfigController)
		{
			_signalService = signalService;
			_carsConfigController = carsConfigController;
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}
		
		protected override void OnMediatorEnable()
		{
			base.OnMediatorEnable();
			
			_data = new UpgradesInfoDataBuffer();

			InitCarInfo();
			InitKits();
			InitCards();
			
		}

		void InitCarInfo()
		{
			_data.carEntity = View.CurrentEntity;
			_data.carLevel = View.CurrentLevel;

			if (_data.carEntity == Entity.Null || !_entityManager.HasComponent<CarIDComponent>(_data.carEntity))
			{
				Debug.LogError("There is no car entity, or car ID component on target entity!");
				return;
			}

			_data.carID = _entityManager.GetComponentData<CarIDComponent>(_data.carEntity).ID;

			_data.carData = _carsConfigController.GetCarData(_data.carID);
		}

		void InitKits()
		{
			var kitsScheme = _entityManager.GetBuffer<KitSchemeBuffer>(_data.carEntity);
			for (int i = 0; i < kitsScheme.Length; i++)
			{
				var platformComponent = _entityManager.GetComponentData<KitPlatformComponent>(kitsScheme[i].Platform);
				var platformConnections = _entityManager.GetBuffer<KitPlatformConnectionBuffer>(kitsScheme[i].Platform);
				var platformConnectedKits = _entityManager.GetBuffer<KitPlatformKitsBuffer>(kitsScheme[i].Platform);

				var platformInfo = new PlatformInfo()
				{
					ID = i,
					isFree = platformComponent.IsFree
				};
				
				foreach (var connection in platformConnections)
				{
					platformInfo.Connections.Add(connection.ConnectionType);
				}

				foreach (var kit in platformConnectedKits)
				{
					var kitComponent = _entityManager.GetComponentData<KitComponent>(kit.ConnectedKit);
					var kitID = _entityManager.GetComponentData<KitIDComponent>(kit.ConnectedKit);
					platformInfo.ConnectedKits.Add(kitComponent);
					platformInfo.ConnectedKitsIDs.Add(kitID);
				}
				
				_data.platformInfos.Add(platformInfo);
			}
		}

		void InitCards()
		{
			int cardsCount = 3;

			var kitsList = GetAllFreeSuitableKits(false);
			kitsList.AddRange(GetAllUpgradeSuitableKits());

			if (kitsList.Count < cardsCount)
			{
				kitsList.AddRange(GetAllFreeSuitableKits(true));
			}

			var rng = new System.Random();
			kitsList = kitsList.OrderBy(a => rng.Next()).ToList();

			cardsCount = Mathf.Clamp(cardsCount, 0, kitsList.Count);

			if (cardsCount == 0)
			{
				View.HideCards();
				View.Complete();
			}

			for (var i = 0; i < cardsCount; i++)
			{
				var kitInfo = kitsList[i];
				var kitData = kitInfo.KitInfo;
				var cardInfo = new UpgradeCardData
				{
					NameKey = kitData.NameLocKey,
					DescriptionKey = kitData.DescriptionLocKey,
					Icon = kitData.Icon,
					Type = kitInfo.Level == 0 ? UpgradeCardType._new : UpgradeCardType._upgrade,
					UpgradeLevel = kitInfo.Level
				};

				View.ShowCard(cardInfo,()=>
				{
					ApplyUpgrade(kitInfo);
				});
			}
		}

		private void ApplyUpgrade(InstallKitInfo info)
		{
			var addKitBuffer = _entityManager.GetBuffer<KitAddBuffer>(_data.carEntity);
			
			addKitBuffer.Add(new KitAddBuffer()
			{
				PlatformID = info.PlatformID,
				CarID = _data.carID,
				KitID = info.KitID,
				KitLevel = info.Level
			});
			
			View.HideCards();
			View.Complete();
		}

		public class InstallKitInfo
		{
			public int PlatformID;
			public int Level;
			public int KitID;
			public KitInfoData KitInfo;
		}

		List<InstallKitInfo> GetAllFreeSuitableKits(bool withDefault)
		{
			var suitableKits = new List<InstallKitInfo>();
			
			foreach (var platform in _data.platformInfos)
			{
				if (!platform.isFree) continue;

				var freeConnectors = platform.Connections;
				for (var i = 0; i < platform.ConnectedKits.Count; i++)
				{
					if(freeConnectors.Contains(platform.ConnectedKits[i].Type))
						freeConnectors.Remove(platform.ConnectedKits[i].Type);
				}
				
				if(freeConnectors.Count == 0) continue;

				for (var i = 0; i < _data.carData.UpgradesConfigs.Count; i++)
				{
					if(_data.carData.UpgradesConfigs[i].isDefault && !withDefault) continue;
					
					if (freeConnectors.Contains(_data.carData.UpgradesConfigs[i].Type))
					{
						suitableKits.Add(new InstallKitInfo()
						{
							PlatformID = platform.ID,
							KitID = i,
							Level = 0,
							KitInfo = _data.carData.UpgradesConfigs[i]
						});
					}
				}
				
			}

			return suitableKits;
		}
		
		List<InstallKitInfo> GetAllUpgradeSuitableKits()
		{
			var suitableKits = new List<InstallKitInfo>();
			
			foreach (var platform in _data.platformInfos)
			{
				for (var i = 0; i < platform.ConnectedKits.Count; i++)
				{
					var kitInfo = _data.carData.UpgradesConfigs[platform.ConnectedKitsIDs[i].ID];
					if (kitInfo.KitObjects.Count > platform.ConnectedKits[i].KitLevel + 1)
						suitableKits.Add(new InstallKitInfo()
						{
							PlatformID = platform.ID,
							Level = platform.ConnectedKits[i].KitLevel + 1,
							KitID = platform.ConnectedKitsIDs[i].ID,
							KitInfo = kitInfo
						});
				}
			}

			return suitableKits;
		}
	}
}