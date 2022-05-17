using Cars.View.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    [UpdateInGroup(typeof(DynamicTerrainSimulationGroup), OrderFirst = true)]
    [UpdateBefore(typeof(DynamicTerrainAddTileSystem))]
    
    public partial class DynamicTerrainTrackingSystem : SystemBase
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
            var trackingInfoBuffer = GetBufferFromEntity<DynamicTerrainTrackInfoData>(true);
            
            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent
            ) =>
            {
                terrainGeneratorEntity = entity;
            }).Run();

            Entities.ForEach((
                Entity entity,
                in CarIDComponent carIDComponent
            ) =>
            {
                if (trackingInfoBuffer.HasComponent(terrainGeneratorEntity))
                {
                    var trackingInfo = trackingInfoBuffer[terrainGeneratorEntity];

                    AddTrackingEntity(ref terrainGeneratorEntity, ref entity, ref trackingInfo, ecbs, true);
                }
            }).WithReadOnly(trackingInfoBuffer).ScheduleParallel();

            Entities.ForEach((
                Entity entity,
                in DynamicTerrainTrackingComponent trackingComponent
            ) =>
            {
                if (trackingInfoBuffer.HasComponent(terrainGeneratorEntity))
                {
                    var trackingInfo = trackingInfoBuffer[terrainGeneratorEntity];
                    
                    AddTrackingEntity(ref terrainGeneratorEntity, ref entity, ref trackingInfo, ecbs, false);
                }
            }).WithReadOnly(trackingInfoBuffer).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void AddTrackingEntity(
            ref Entity generatorEntity,
            ref Entity trackingEntity,
            ref DynamicBuffer<DynamicTerrainTrackInfoData> trackingInfoBuffer,
            EntityCommandBuffer.ParallelWriter ecbs,
            bool isPlayer
        )
        {
            var isNotAdd = false;

            for (var i = 0; i < trackingInfoBuffer.Length; i++)
            {
                var currentEntity = trackingInfoBuffer[i].TrackEntity;
              
                if (trackingEntity.Index == currentEntity.Index)
                    isNotAdd = true;
            }

            if (!isNotAdd)
                ecbs.AppendToBuffer(0, generatorEntity, new DynamicTerrainTrackInfoData()
                {
                    TrackEntity = trackingEntity,
                    IsPlayer = isPlayer
                });
        }
    }
}
