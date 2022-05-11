using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace StatisticsSystem.Systems
{
    [UpdateAfter(typeof(RecalculateCharacteristicsSystem))]
    public partial class CharacteristicsSynchronizationSystem : SystemBase
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
                GetComponentDataFromEntity<CharacteristicsComponent>(true);
            var childFilter = GetBufferFromEntity<Child>(true);
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.WithAll<NewCharacteristicsTag, Child>().ForEach(
                (Entity entity, in CharacteristicsComponent statisticsComponent) =>
                {
                    UpdateStatistics(entity, statisticsComponent, statisticsComponentFilter, childFilter, ecb);

                    ecb.RemoveComponent<NewCharacteristicsTag>(entity);
                }).WithReadOnly(childFilter).WithReadOnly(statisticsComponentFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        static void UpdateStatistics(Entity entity, in CharacteristicsComponent characteristicsComponent,
            ComponentDataFromEntity<CharacteristicsComponent> statisticsComponentFilter, BufferFromEntity<Child> childFilter,
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
                    ecb.SetComponent(childEntity, characteristicsComponent);
                    ecb.AddComponent(childEntity, new CharacteristicsUpdatedTag());
                }

                if (childFilter.HasComponent(childEntity))
                {
                    UpdateStatistics(childEntity, characteristicsComponent, statisticsComponentFilter, childFilter, ecb);
                }
            }
        }
    }
}