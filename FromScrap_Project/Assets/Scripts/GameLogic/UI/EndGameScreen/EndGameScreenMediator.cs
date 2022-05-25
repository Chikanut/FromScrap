using Packages.Common.Storage.Config;
using ShootCommon.Views.Mediation;
using Signals;
using UnityEngine;
using Zenject;

namespace UI.Screens.Loading
{
    public class EndGameScreenMediator : Mediator<EndGameScreenView>
    {
        private IGameDataController _gameDataController;
        private IPlayerProgressionConfigController _playerProgressionConfigController;
        
        [Inject]
        public void Init(IGameDataController gameDataController, IPlayerProgressionConfigController playerProgressionConfigController)
        {
            _gameDataController = gameDataController;
            _playerProgressionConfigController = playerProgressionConfigController;
        }
        
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

           View.OnMainMenuAction = OnMainMenuAction;
        }
        
        protected override void OnMediatorEnable()
        {
            base.OnMediatorEnable();
            
            View.UpdateInfo(_gameDataController.Data.CarData);
            View.UpdateStats(_gameDataController.Data.Stats);
            View.UpdateScrap(100);//_gameDataController.Data.Stats.CollectedScrap);
            View.UpdateXP(_gameDataController.Data.Stats.ExperienceGained,
                _playerProgressionConfigController.GetPlayerProgressionData);
        }
        
        private void OnMainMenuAction()
        {
            SignalService.Publish(new GoToMainMenuSignal());
        }
    }
}