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
		private EntityManager _entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;
		private IGameDataController _gameDataController;
		private IUpgradesConfigController _upgradesConfigController;

		private CurrentCarInfoData _data;

		[Inject]
		public void Init(IGameDataController gameDataController, IUpgradesConfigController upgradesConfigController)
		{
			_gameDataController = gameDataController;
			_upgradesConfigController = upgradesConfigController;
		}
		
		protected override void OnMediatorEnable()
		{
			base.OnMediatorEnable();
			
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
				KitIndex = info.KitIndex,
				KitLevel = info.Level
			});
			
			View.HideCards();
			View.Complete();
		}

		public class InstallKitInfo
		{
			public int PlatformID;
			public int Level;
			public int KitIndex;
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

				for (var i = 0; i < _upgradesConfigController.GetUpgradesData.Kits.Count; i++)
				{
					if(_upgradesConfigController.GetUpgradesData.Kits[i].Data.isDefault && !withDefault) continue;
					
					if (freeConnectors.Contains(_upgradesConfigController.GetUpgradesData.Kits[i].Data.Type) && platform.ConnectedKitsIndexes.All(k => k.Index != i))
					{
						suitableKits.Add(new InstallKitInfo()
						{
							PlatformID = platform.ID,
							KitIndex = i,
							Level = 0,
							KitInfo = _upgradesConfigController.GetUpgradesData.Kits[i].Data
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
					var kitInfo = _upgradesConfigController.GetUpgradesData.Kits[platform.ConnectedKitsIndexes[i].Index].Data;
					if (kitInfo.KitObjects.Count > platform.ConnectedKits[i].KitLevel + 1)
						suitableKits.Add(new InstallKitInfo()
						{
							PlatformID = platform.ID,
							Level = platform.ConnectedKits[i].KitLevel + 1,
							KitIndex = platform.ConnectedKitsIndexes[i].Index,
							KitInfo = kitInfo
						});
				}
			}

			return suitableKits;
		}
	}
}