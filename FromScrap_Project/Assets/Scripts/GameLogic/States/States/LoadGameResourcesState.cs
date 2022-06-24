using GameLogic.GameResourcesLogic;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using Zenject;

namespace GameLogic.States.States
{
    public class LoadGameResourcesState : GlobalState
    {
        private IGameResourcesLoaderController _gameResourcesLoaderController;
        
        protected override void Configure()
        {
            Permit<InitGameState>(StateMachineTriggers.InitGame);
        }
        
        [Inject]
        public void Init(IGameResourcesLoaderController gameResourcesLoaderController)
        {
            _gameResourcesLoaderController = gameResourcesLoaderController;
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignals();
            
            _gameResourcesLoaderController.LoadAssets();
        }

        private void SubscribeToSignals()
        {
            SubscribeToSignal<AllResourcesReadySignal>((signal) =>
            {
                Fire(StateMachineTriggers.InitGame);
            });
        }
    }
}