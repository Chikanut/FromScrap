using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace StatisticsSystem.Systems
{
    [UpdateBefore(typeof(RecalculateStatisticsSystem))]
    public partial class ApplyStatisticsModificationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var parentFilter = GetComponentDataFromEntity<Parent>(true);
            var modificationsBufferFilter = GetBufferFromEntity<StatisticModificationsBuffer>(true);

            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.ForEach((Entity entity, in StatisticsModificationComponent modificationComponent) =>
            {
                ApplyModification(entity, entity, modificationComponent, parentFilter, modificationsBufferFilter, ecb);

                ecb.RemoveComponent<StatisticsModificationComponent>(entity);
            }).WithReadOnly(parentFilter).WithReadOnly(modificationsBufferFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void ApplyModification(Entity entity, Entity holder,
            in StatisticsModificationComponent modificationComponent, ComponentDataFromEntity<Parent> parentsFilter,
            BufferFromEntity<StatisticModificationsBuffer> modificationsBufferFilter, EntityCommandBuffer ecb)
        {
            if (!parentsFilter.HasComponent(entity))
                return;

            var parent = parentsFilter[entity].Value;

            if (modificationsBufferFilter.HasComponent(parent))
            {
                ecb.AppendToBuffer(parent,
                    new StatisticModificationsBuffer()
                        {Value = modificationComponent.Value, ModificatorHolder = holder});
                ecb.AddComponent(parent, new StatisticsUpdatedTag());
            }
            else
            {
                ApplyModification(parent, holder, modificationComponent, parentsFilter, modificationsBufferFilter, ecb);
            }
        }
    }
}