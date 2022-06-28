using ECS.SignalSystems.Systems;
using GameLogic.GameResourcesLogic;
using GameLogic.GameResourcesLogic.Controllers;
using ShootCommon.GlobalStateMachine;
using Stateless;
using Zenject;

namespace GameLogic.States.States
{
    public class LoadGameResourcesState : GlobalState
    {
        private IGameResourcesLoaderController _gameResourcesLoaderController;
        
        protected override void Configure()
        {
            Permit<SpawnGameResourcesState>(StateMachineTriggers.SpawnGameResources);
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
            SubscribeToSignal<OnGameResourcesLoadedSignal>(OnGameResourcesLoaded);
        }
        
        void OnGameResourcesLoaded(OnGameResourcesLoadedSignal signal)
        {
            Fire(StateMachineTriggers.SpawnGameResources);
        }
    }
}