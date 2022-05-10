using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace StatisticsSystem.Systems
{
    [UpdateAfter(typeof(RecalculateStatisticsSystem))]
    public partial class InitializeStatisticSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var statisticsComponentFilter =
                GetComponentDataFromEntity<StatisticsComponent>(true);
            var parentFilter = GetComponentDataFromEntity<Parent>(true);
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.WithAll<GetStatisticTag, StatisticsComponent, Parent>().ForEach(
                (Entity entity) =>
                {
                    UpdateStatistics(entity, statisticsComponentFilter, parentFilter, ecb);

                    ecb.RemoveComponent<GetStatisticTag>(entity);
                }).WithReadOnly(parentFilter).WithReadOnly(statisticsComponentFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static void UpdateStatistics(Entity entity, ComponentDataFromEntity<StatisticsComponent> statisticsComponentFilter, ComponentDataFromEntity<Parent> parentFilter, EntityCommandBuffer ecb)
        {
            if (!parentFilter.HasComponent(entity))
                return;

            var parent = parentFilter[entity].Value;
            
            if (statisticsComponentFilter.HasComponent(parent))
            {
                ecb.SetComponent(entity, statisticsComponentFilter[parent]);
            }
            else if(parentFilter.HasComponent(parent))
            {
                UpdateStatistics(parent, statisticsComponentFilter, parentFilter, ecb);
            }
        }
    }
}