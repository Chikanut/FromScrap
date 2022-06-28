using GameLogic.GameResourcesLogic.Controllers;
using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using Zenject;

namespace GameLogic.GameResourcesLogic.GameResourcesSceneController
{
    public class GameResourcesSceneMediator : Mediator<GameResourcesSceneView>
    {
        private ISignalService _signalService;
        private IGameResourcesLoaderController _gameResourcesLoaderController;

        [Inject]
        public void Init(ISignalService signalService, IGameResourcesLoaderController gameResourcesLoaderController)
        {
            _signalService = signalService;
            _gameResourcesLoaderController = gameResourcesLoaderController;
        }

        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

        }

        protected override void OnMediatorEnable()
        {
            base.OnMediatorEnable();

        }

        protected override void OnMediatorDispose()
        {
            base.OnMediatorDispose();
            _gameResourcesLoaderController.Dispose();
        }
    }
}