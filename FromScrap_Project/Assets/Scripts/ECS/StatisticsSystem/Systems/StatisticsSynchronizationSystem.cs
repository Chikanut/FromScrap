using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace StatisticsSystem.Systems
{
    [UpdateAfter(typeof(RecalculateStatisticsSystem))]
    public partial class StatisticsSynchronizationSystem : SystemBase
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
            var childFilter = GetBufferFromEntity<Child>(true);
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.WithAll<StatisticsUpdatedTag, Child>().ForEach(
                (Entity entity, in StatisticsComponent statisticsComponent) =>
                {
                    UpdateStatistics(entity, statisticsComponent, statisticsComponentFilter, childFilter, ecb);

                    ecb.RemoveComponent<StatisticsUpdatedTag>(entity);
                }).WithReadOnly(childFilter).WithReadOnly(statisticsComponentFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static void UpdateStatistics(Entity entity, in StatisticsComponent statisticsComponent,
            ComponentDataFromEntity<StatisticsComponent> statisticsComponentFilter, BufferFromEntity<Child> childFilter,
            EntityCommandBuffer ecb)
        {
            if (!childFilter.HasComponent(entity))
                return;

            var children = childFilter[entity];

            for (int i = 0; i < children.Length; i++)
            {
                var childEntity = children[i].Value;

                if (statisticsComponentFilter.HasComponent(childEntity))
                {
                    ecb.SetComponent(childEntity, statisticsComponent);
                }

                if (childFilter.HasComponent(childEntity))
                {
                    UpdateStatistics(childEntity, statisticsComponent, statisticsComponentFilter, childFilter, ecb);
                }
            }
        }
    }
}