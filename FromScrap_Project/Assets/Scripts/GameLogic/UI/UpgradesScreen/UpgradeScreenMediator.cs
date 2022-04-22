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
					platformInfo.ConnectedKits.Add(kitComponent);
				}
				
				_data.platformInfos.Add(platformInfo);
			}
		}

		void InitCards()
		{
			int cardsCount = 3;

			var kitsList = GetAllFreeSuitableKits();
			kitsList.AddRange(GetAllUpgradeSuitableKits());
			
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
				var kitData = _data.carData.UpgradesConfigs[kitInfo.kitID];
				var cardInfo = new UpgradeCardData
				{
					NameKey = kitData.NameLocKey,
					DescriptionKey = kitData.DescriptionLocKey,
					Icon = kitData.Icon,
					Type = kitInfo.level == 0 ? UpgradeCardType._new : UpgradeCardType._upgrade,
					UpgradeLevel = kitInfo.level
				};

				View.ShowCard(cardInfo,()=>
				{
					ApplyUpgrade(kitInfo);
				});
			}
		}

		private void ApplyUpgrade((int platform, int level, int kitID) info)
		{
			var addKitBuffer = _entityManager.GetBuffer<KitAddBuffer>(_data.carEntity);
			
			addKitBuffer.Add(new KitAddBuffer()
			{
				PlatformID = info.platform,
				CarID = _data.carID,
				KitID = info.kitID,
				KitLevel = info.level
			});
			
			View.HideCards();
			View.Complete();
		}

		List<(int platformID, int level, int kitID)> GetAllFreeSuitableKits()
		{
			var suitableKits = new List<(int platformID, int kitLevel, int kitID)>();

			var platformID = 0;
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
					if (freeConnectors.Contains(_data.carData.UpgradesConfigs[i].Type))
					{
						suitableKits.Add((platformID, 0, i));
					}
				}

				platformID++;
			}

			return suitableKits;
		}
		
		List<(int platformID, int kitLevel, int kitID)> GetAllUpgradeSuitableKits()
		{
			var suitableKits = new List<(int platformID, int kitLevel, int kitID)>();
			
			var platformID = 0;
			foreach (var platform in _data.platformInfos)
			{
				for (var i = 0; i < platform.ConnectedKits.Count; i++)
				{
					var kitInfo = _data.carData.UpgradesConfigs[platform.ConnectedKits[i].ID];
					if (kitInfo.KitObjects.Count > platform.ConnectedKits[i].KitLevel + 1)
						suitableKits.Add((platformID, platform.ConnectedKits[i].KitLevel + 1, platform.ConnectedKits[i].ID));
				}

				platformID++;
			}

			return suitableKits;
		}
	}
}