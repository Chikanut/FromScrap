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

			var kitsList = UpgradesSelectClass.GetSuitableKits(cardsCount, _upgradesConfigController, _data);

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
					UpgradeLevel = kitInfo.Level,
					UpgradeMaxLevel = kitData.KitObjects.Count
				};

				View.ShowCard(cardInfo,()=>
				{
					ApplyUpgrade(kitInfo);
				});
			}
		}

		private void ApplyUpgrade(UpgradesSelectClass.InstallKitInfo info)
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
	}
}