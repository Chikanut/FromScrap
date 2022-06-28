using Configs.Gameplay.Controllers;
using ECS.SignalSystems.Systems;
using GameLogic.GameResourcesLogic;
using GameLogic.GameResourcesLogic.Controllers;
using ShootCommon.GlobalStateMachine;
using Stateless;
using Zenject;

namespace GameLogic.States.States
{
    public class SpawnGameResourcesState : GlobalState
    {
        private IGameResourcesSpawnerController _gameResourcesSpawnerController;
        
        protected override void Configure()
        {
            Permit<InitGameState>(StateMachineTriggers.InitGame);
        }
        
        [Inject]
        public void Init(IGameResourcesSpawnerController gameResourcesSpawnerController)
        {
            _gameResourcesSpawnerController = gameResourcesSpawnerController;
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignals();
            
            _gameResourcesSpawnerController.SpawnGameLevelAssets();
        }

        private void SubscribeToSignals()
        {
            SubscribeToSignal<OnGameResourcesSpawnedSignal>(OnGameResourcesSpawned);
        }
        
        void OnGameResourcesSpawned(OnGameResourcesSpawnedSignal signal)
        {
            Fire(StateMachineTriggers.InitGame);
        }
    }
}