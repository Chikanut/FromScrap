using Cars.View.Components;
using DamageSystem.Components;
using DG.Tweening;
using MenuNavigation;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.GlobalStateMachine;
using Stateless;
using UI.Loading;
using Unity.Mathematics;
using Unity.Transforms;
using Visartech.Progress;
using Zenject;

namespace GameLogic.States.States
{
    public class InitGameState : GlobalState
    {
        protected override void Configure()
        {
            Permit<GameplayState>(StateMachineTriggers.Game);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignals();
            InitGame();
            
            _menuNavigationController.HideMenuScreen<LoadingScreenView>(null, "LoadingScreen");
        }
        
        private void SubscribeToSignals()
        {
            
        }
        
        private ICarsConfigController _carsConfigController;
        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(ICarsConfigController carsConfigController, IMenuNavigationController menuNavigationController)
        {
            _carsConfigController = carsConfigController;
            _menuNavigationController = menuNavigationController;
        }

        void InitGame()
        {
            SpawnPlayer();
 
            //TODO: Wait till all game parts is initialized
            _menuNavigationController.ShowMenuScreen<GamePlayScreenView>(() =>
            {
                Fire(StateMachineTriggers.Game);
            }, "GamePlayScreen");
        }

        void SpawnPlayer()
        {
            var carData = _carsConfigController.GetCarData(Progress.Player.CurrentCar);
        
            EntityPoolManager.Instance.GetObject(carData.Prefab, (entity, manager) =>
            {
                manager.AddComponentData(entity, new CarIDComponent() {ID = Progress.Player.CurrentCar});
                manager.AddComponentData(entity, new PlayerTag());
                manager.SetComponentData(entity, new Translation()
                {
                    Value = new float3(0,3,0)
                });
                var health = manager.GetComponentData<Health>(entity);
                health.Value = health.InitialValue = carData.Health;
                manager.SetComponentData(entity, health);
            });
        }

    }
}