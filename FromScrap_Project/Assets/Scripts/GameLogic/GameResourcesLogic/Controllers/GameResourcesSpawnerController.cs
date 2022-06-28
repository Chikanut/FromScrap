using ECS.GameResourcesSystem.Components;
using Serializable.ResourcesManager;
using Configs.Gameplay.Controllers;
using ShootCommon.Signals;
using Unity.Entities;
using Zenject;

namespace GameLogic.GameResourcesLogic.Controllers
{
    public class GameResourcesSpawnerController : IGameResourcesSpawnerController, IInitializable
    {
        private IGameplayLevelsConfigController _gameplayLevelsConfigController;
        private ISignalService _signalService;

        [Inject]
        public void Init(IGameplayLevelsConfigController gameplayLevelsConfigController,
            ISignalService signalService)
        {
            _gameplayLevelsConfigController = gameplayLevelsConfigController;
            _signalService = signalService;
        }

        public virtual void SpawnGameLevelAssets()
        {
            //--> Spawn game resources spawn system component

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var levelConfig = _gameplayLevelsConfigController.GetGameplayLevelsData.GameplayLevels;
            var commonAssets = levelConfig.CommonGameLevelsComponents;

            foreach (var commonAsset in commonAssets)
            {
                if (commonAsset == CommonGameLevelsComponent.DynamicTerrain)
                {
                    var spawnEntity = entityManager.CreateEntity();

                    entityManager.AddComponentData(spawnEntity, new GameResourcesSpawnEntityId()
                    {
                        TypeId = GameResourcesEntityTypeId.DynamicTerrain
                    });
                }
            }
        }

        public virtual void Dispose()
        {
            //TODO: dispose logic and data here

        }

        public void Initialize()
        {

        }
    }
}