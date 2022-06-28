using Configs.GameResourcesManagerConfig.Configs;
using Configs.GameResourcesManagerConfig.Controllers;
using ECS.GameResourcesSystem.Components;
using GameLogic.GameResourcesLogic.GameResourcesSceneController;
using Serializable.ResourcesManager;
using ShootCommon.Signals;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace GameLogic.GameResourcesLogic.Controllers
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
           
        }

        public virtual void LoadAssets()
        {
            _blobAssetStore = new BlobAssetStore();
            
            //--> Create game resources scene controller

            var gameResourcesSceneController = new GameObject("Game Resources Scene Controller");
            gameResourcesSceneController.AddComponent<GameResourcesSceneView>();
            
            //--> Create game resources spawn system component

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var gameResourcesSpawnEntity = entityManager.CreateEntity();
          
            entityManager.SetName(gameResourcesSpawnEntity, "Game Resources Spawner");
            entityManager.AddComponentData(gameResourcesSpawnEntity, new GameResourcesSpawnComponent() { });

            //--> Load core gameplay assets

            var coreGameplayAsset = _gameResourcesManagerConfigController.GetGameResourcesData.CoreGameplayAsset;
            coreGameplayAsset.ReleaseAsset();
            var coreGameplayAssetHandle = coreGameplayAsset.LoadAssetAsync<CoreGameplayAssetsConfig>();
            
            coreGameplayAssetHandle.Completed += coreGameplayAssetOperationHandle =>
            {
                if (coreGameplayAssetOperationHandle.OperationException == null)
                {
                    var dynamicTerrainAsset = coreGameplayAssetOperationHandle.Result.DynamicTerrainAsset;
                    var dynamicTerrainAssetHandle = dynamicTerrainAsset.LoadAssetAsync<GameObject>();

                    dynamicTerrainAssetHandle.Completed += dynamicTerrainAssetOperationHandle =>
                    {
                        if (dynamicTerrainAssetOperationHandle.OperationException == null)
                        {
                            var dynamicTerrainObject = dynamicTerrainAssetOperationHandle.Result;
                            var conversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
                            var dynamicTerrainEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(dynamicTerrainObject.gameObject, conversionSettings);
                            var dynamicTerrainSendEntity = entityManager.CreateEntity();

                            entityManager.AddComponentData(dynamicTerrainSendEntity, new GameResourcesLoadEntityId { });
                            entityManager.SetComponentData(dynamicTerrainSendEntity, new GameResourcesLoadEntityId()
                            {
                                TargetEntity = dynamicTerrainEntity,
                                TypeId = GameResourcesEntityTypeId.DynamicTerrain
                            });
                        }
                    };
                }
            };
        }

        public virtual void Dispose()
        {
            _blobAssetStore.Dispose();
        }

        public void Initialize()
        {

        }
    }
}