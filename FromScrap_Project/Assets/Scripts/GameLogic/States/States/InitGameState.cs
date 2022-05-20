using Cars.View.Components;
using DamageSystem.Components;
using MenuNavigation;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.GlobalStateMachine;
using Stateless;
using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Steamworks;
using UI.Screens.Loading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
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
                    Value = new float3(0,45,0)
                });
                var health = manager.GetComponentData<Health>(entity);
                health.SetMaxHealth(carData.BaseCharacteristics.MaxHealth, true);
                manager.SetComponentData(entity, health);
                
                var modifications = manager.GetBuffer<CharacteristicModificationsBuffer>(entity);
                modifications.Add(new CharacteristicModificationsBuffer {Value = carData.BaseCharacteristics, ModificatorHolder = entity});
                manager.AddComponentData(entity, new NewCharacteristicsTag());
            });
        }

    }
}