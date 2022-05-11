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

            Dependency = Entities.WithAll<StatisticsComponent, Parent>().ForEach(
                (Entity entity, ref GetStatisticTag getStatisticTag) =>
                {
                    var updated = UpdateStatistics(entity, entity, statisticsComponentFilter, parentFilter, ecb);

                   if(updated || getStatisticTag.TryUpdateTimes >= GetStatisticTag.MaxTryUpdateTimes)  
                       ecb.RemoveComponent<GetStatisticTag>(entity);
                   else
                       getStatisticTag.TryUpdateTimes++;

                }).WithReadOnly(parentFilter).WithReadOnly(statisticsComponentFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static bool UpdateStatistics(Entity entity, Entity handler, ComponentDataFromEntity<StatisticsComponent> statisticsComponentFilter, ComponentDataFromEntity<Parent> parentFilter, EntityCommandBuffer ecb)
        {
            if (!parentFilter.HasComponent(entity))
                return false;

            var parent = parentFilter[entity].Value;

            if (!statisticsComponentFilter.HasComponent(parent))
                return parentFilter.HasComponent(parent) && UpdateStatistics(parent, handler, statisticsComponentFilter, parentFilter, ecb);
            
            ecb.SetComponent(handler, statisticsComponentFilter[parent]);
            return true;

        }
    }
}