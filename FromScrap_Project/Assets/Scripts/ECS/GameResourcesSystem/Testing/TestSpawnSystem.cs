using ECS.GameResourcesSystem.Components;
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
            /*
            var ecbs = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref GameResourcesSpawnComponent testSpawnComponent,
                ref TestSpawnLoadComponent testSpawnLoadComponent
            ) =>
            {
                if(testSpawnComponent.EnableSpawn)
                    SpawnTest(ref testSpawnComponent, ref testSpawnLoadComponent, ecbs, entityInQueryIndex);
            }).ScheduleParallel(Dependency);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
            */
        }
        
        /*
        private static void SpawnTest(
            ref GameResourcesSpawnComponent gameResourcesSpawnComponent,
            ref TestSpawnLoadComponent testSpawnLoadComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            int entityInQueryIndex
        )
        {
            gameResourcesSpawnComponent.EnableSpawn = false;
            
            for (var i = 0; i < gameResourcesSpawnComponent.EntityCount; i++)
            {
                var tileEntity = ecbs.Instantiate(entityInQueryIndex, testSpawnLoadComponent.SpawnEntity);

                ecbs.AddComponent(entityInQueryIndex, tileEntity, new Translation()
                {
                    Value = new float3(i, 0f, 0f)
                });
            }
        }
        */
    }
}
