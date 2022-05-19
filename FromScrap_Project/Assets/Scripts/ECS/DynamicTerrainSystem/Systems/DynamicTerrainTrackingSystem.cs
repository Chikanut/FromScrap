using Cars.View.Components;
using Unity.Entities;

namespace ECS.DynamicTerrainSystem
{
    [UpdateInGroup(typeof(DynamicTerrainSimulationGroup), OrderFirst = true)]

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

            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                in CarIDComponent carIDComponent
            ) =>
            {
                if (trackingInfoBuffer.HasComponent(terrainGeneratorEntity))
                {
                    var trackingInfo = trackingInfoBuffer[terrainGeneratorEntity];

                    AddTrackingEntity(
                        ref terrainGeneratorEntity, 
                        ref entity, 
                        ref trackingInfo, 
                        ecbs, 
                        entityInQueryIndex,
                        true);
                }
            }).WithReadOnly(trackingInfoBuffer).ScheduleParallel(Dependency);

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
            
            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                in DynamicTerrainTrackingComponent trackingComponent
            ) =>
            {
                if (trackingInfoBuffer.HasComponent(terrainGeneratorEntity))
                {
                    var trackingInfo = trackingInfoBuffer[terrainGeneratorEntity];
                    
                    AddTrackingEntity(
                        ref terrainGeneratorEntity, 
                        ref entity,
                        ref trackingInfo,
                        ecbs,
                        entityInQueryIndex,
                        false);
                }
            }).WithReadOnly(trackingInfoBuffer).ScheduleParallel(Dependency);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void AddTrackingEntity(
            ref Entity generatorEntity,
            ref Entity trackingEntity,
            ref DynamicBuffer<DynamicTerrainTrackInfoData> trackingInfoBuffer,
            EntityCommandBuffer.ParallelWriter ecbs,
            int entityInQueryIndex,
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
                ecbs.AppendToBuffer(entityInQueryIndex, generatorEntity, new DynamicTerrainTrackInfoData()
                {
                    TrackEntity = trackingEntity,
                    IsPlayer = isPlayer
                });
        }
    }
}
