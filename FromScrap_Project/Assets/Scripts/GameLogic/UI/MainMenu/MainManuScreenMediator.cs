using Packages.Common.Storage.Config;
using ShootCommon.Views.Mediation;
using UniRx;
using Visartech.Progress;
using Zenject;

namespace UI.Screens.MainMenu
{
    public class MainManuScreenMediator : Mediator<MainMenuScreenView>
    {
        private IPlayerProgressionConfigController _playerProgressionConfigController;
        
        [Inject]
        public void Init(IPlayerProgressionConfigController playerProgressionConfigController)
        {
            _playerProgressionConfigController = playerProgressionConfigController;
        }

        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

            SignalService.Receive<OnNewUpdateAction>().Subscribe(OnNewUpdated).AddTo(DisposeOnDestroy);
            SignalService.Receive<OnPlayerInfoChanged>().Subscribe(OnPlayerInfoChanged).AddTo(DisposeOnDestroy);
        }

        protected override void OnMediatorEnable()
        {
            base.OnMediatorEnable();

            View.InitPlayerInfo(Progress.Player.Level,
                (float)Progress.Player.Experience /
                _playerProgressionConfigController.GetPlayerProgressionData.LevelsExperience[Progress.Player.Level],
                Progress.Player.Scrap);
        }

        public void OnPlayerInfoChanged(OnPlayerInfoChanged signal)
        {
            View.InitPlayerInfo(Progress.Player.Level,
                (float)Progress.Player.Experience /
                _playerProgressionConfigController.GetPlayerProgressionData.LevelsExperience[Progress.Player.Level],
                Progress.Player.Scrap);
        }

        void OnNewUpdated(OnNewUpdateAction action)
        {
            View.CheckNew();
        }
    }
}