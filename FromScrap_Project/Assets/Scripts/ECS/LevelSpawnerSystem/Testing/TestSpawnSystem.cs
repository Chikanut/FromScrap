using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.LevelSpawnerSystem
{
    public partial class TestSpawnSystem : SystemBase
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
            
            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref TestSpawnComponent testSpawnComponent,
                ref TestSpawnLoadComponent testSpawnLoadComponent
            ) =>
            {
                if(testSpawnComponent.EnableSpawn)
                    SpawnTest(ref testSpawnComponent, ref testSpawnLoadComponent, ecbs, entityInQueryIndex);
            }).ScheduleParallel(Dependency);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
        
        private static void SpawnTest(
            ref TestSpawnComponent testSpawnComponent,
            ref TestSpawnLoadComponent testSpawnLoadComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            int entityInQueryIndex
        )
        {
            testSpawnComponent.EnableSpawn = false;
            
            for (var i = 0; i < testSpawnComponent.EntityCount; i++)
            {
                var tileEntity = ecbs.Instantiate(entityInQueryIndex, testSpawnLoadComponent.SpawnEntity);

                ecbs.AddComponent(entityInQueryIndex, tileEntity, new Translation()
                {
                    Value = new float3(i, 0f, 0f)
                });
            }
        }
    }
}
