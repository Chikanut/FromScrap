using Configs.GameResourcesManagerConfig.Controllers;
using ECS.LevelSpawnerSystem;
using ShootCommon.Signals;
using Signals;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace GameLogic.GameResourcesLogic
{
    public class GameResourcesLoaderController : IGameResourcesLoaderController, IInitializable
    {
        private IGameResourcesManagerConfigController _gameResourcesManagerConfigController;
        private ISignalService _signalService;

        private BlobAssetStore _blobAssetStore;

        [Inject]
        public void Init(IGameResourcesManagerConfigController gameResourcesManagerConfigController,
            ISignalService signalService)
        {
            _gameResourcesManagerConfigController = gameResourcesManagerConfigController;
            _signalService = signalService;

            //TODO: Recdeive start signal
            //SignalService.Receive<OnExperienceChangeSignal>().Subscribe(OnExperienceGathered).AddTo(DisposeOnDestroy);
        }

        public virtual void LoadAssets()
        {
            //TODO: Init view game resources manager here

            var testADPrefab = _gameResourcesManagerConfigController.GetGameResourcesData.UIPrefabAsset;

            AsyncOperationHandle<GameObject> goHandle = testADPrefab.LoadAssetAsync<GameObject>();
            goHandle.Completed += operationHandle =>
            {
                //var MyPrefab = Instantiate(goHandle.Result);
                var MyPrefab = goHandle.Result;
                _blobAssetStore = new BlobAssetStore();
                GameObjectConversionSettings settings =
                    GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
                var MyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(MyPrefab.gameObject, settings);
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;



                Entity entity = entityManager.CreateEntity();

                entityManager.AddComponentData(entity, new TestSpawnElementId { });

                entityManager.SetComponentData(entity, new TestSpawnElementId() {TargetEntity = MyEntity});

                _signalService.Publish(new AllResourcesReadySignal());
                Dispose();
            };
        }

        public virtual void Dispose()
        {
            //TODO: dispose logic and data here

            _blobAssetStore.Dispose();
        }

        public void Initialize()
        {

        }
    }
}