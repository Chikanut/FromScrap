using Packages.Common.Storage.Config.Upgrades;
using ShootCommon.Views.Mediation;
using Signals;
using UnityEngine;
using Zenject;

namespace UI.PopUps.Pause
{
    public class PausePopUpMediator : Mediator<PausePopUpView>
    {
        private IGameDataController _gameDataController;
        private IUpgradesConfigController _upgradesConfigController;
        
        [Inject]
        public void Init(IGameDataController gameDataController, IUpgradesConfigController upgradesConfigController)
        {
            _gameDataController = gameDataController;
            _upgradesConfigController = upgradesConfigController;
        }

        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

            View.OnContinueAction = OnContinueAction;
            View.OnMainMenuAction = OnMainMenuAction;
            View.OnRestartAction = OnRestartAction;
        }

        protected override void OnMediatorEnable()
        {
            base.OnMediatorEnable();
            View.UpdateInfo(_gameDataController.Data.CarData, _upgradesConfigController.GetUpgradesData);
        }

        private void OnContinueAction()
        {
            SignalService.Publish(new ContinueGameSignal());
        }

        private void OnMainMenuAction()
        {
            SignalService.Publish(new GoToMainMenuSignal());
        }

        private void OnRestartAction()
        {
            SignalService.Publish(new RestartGameSignal());
        }
    }
}