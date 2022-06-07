using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ECS.LevelSpawnerSystem
{
    public partial class TestSpawnLoadSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbs = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var terrainGeneratorEntity = Entity.Null;

            Entities.ForEach((
                Entity entity,
                ref TestSpawnComponent testSpawnComponent
            ) =>
            {
                terrainGeneratorEntity = entity;
            }).Run();

            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                in TestSpawnElementId testSpawnElementId
            ) =>
            {
               ecbs.AddComponent(0, terrainGeneratorEntity, new TestSpawnLoadComponent
               {
                   SpawnEntity = testSpawnElementId.TargetEntity
               }); 
            }).ScheduleParallel(Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}