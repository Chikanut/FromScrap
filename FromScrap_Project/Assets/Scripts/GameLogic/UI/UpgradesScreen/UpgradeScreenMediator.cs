using System.Collections.Generic;
using System.Linq;
using Kits.Components;
using Packages.Common.Storage.Config.Upgrades;
using ShootCommon.Views.Mediation;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace UI.Screens.Upgrades
{
	public class UpgradeScreenMediator : Mediator<UpgradeScreenView>
	{
		private EntityManager _entityManager;
		private IGameDataController _gameDataController;

		private CurrentCarInfoData _data;

		[Inject]
		public void Init(IGameDataController gameDataController)
		{
			_gameDataController = gameDataController;
		}
		
		protected override void OnMediatorEnable()
		{
			base.OnMediatorEnable();

			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			_data = _gameDataController.Data.CarData;
	
			InitCards();
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
					Descriptions = kitData.KitObjects[kitInfo.Level].Descriptions,
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
				if (platform.canOccupy)
				{
					for (var i = 0; i < platform.ConnectedKits.Count; i++)
					{
						if (freeConnectors.Contains(platform.ConnectedKits[i].Type))
							freeConnectors.Remove(platform.ConnectedKits[i].Type);
					}
				}

				if(freeConnectors.Count == 0) continue;

				for (var i = 0; i < _data.carData.UpgradesConfigs.Count; i++)
				{
					if(_data.carData.UpgradesConfigs[i].isDefault && !withDefault) continue;
					
					if (freeConnectors.Contains(_data.carData.UpgradesConfigs[i].Type) && platform.ConnectedKitsIDs.All(k => k.ID != i))
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