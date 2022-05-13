using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;

namespace StatisticsSystem.Systems
{
    [UpdateAfter(typeof(RecalculateCharacteristicsSystem)), UpdateAfter(typeof(InitializeCharacteristicsSystem))]
    public partial class LocalCharacteristicsSynchronizationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var localStatisticsFilter = GetComponentDataFromEntity<LocalCharacteristicsComponent>(true);
            
            Dependency = Entities.WithAll<CharacteristicsUpdatedTag>().ForEach((Entity entity, int entityInQueryIndex, ref CharacteristicsComponent stats) =>
            {
                if (localStatisticsFilter.HasComponent(entity))
                    stats.Value.Add(localStatisticsFilter[entity].Value);

                ecb.RemoveComponent<CharacteristicsUpdatedTag>(entityInQueryIndex, entity);
            }).WithReadOnly(localStatisticsFilter).ScheduleParallel(Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}