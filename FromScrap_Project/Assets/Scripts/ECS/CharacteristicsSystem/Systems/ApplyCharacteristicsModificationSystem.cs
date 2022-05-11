using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using Unity.Transforms;

namespace StatisticsSystem.Systems
{
    [UpdateBefore(typeof(RecalculateCharacteristicsSystem))]
    public partial class ApplyCharacteristicsModificationSystem : SystemBase
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
            var modificationsBufferFilter = GetBufferFromEntity<CharacteristicModificationsBuffer>(true);

            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Dependency = Entities.ForEach((Entity entity, in CharacteristicsModificationComponent modificationComponent) =>
            {
                ApplyModification(entity, entity, modificationComponent, parentFilter, modificationsBufferFilter, ecb);

                ecb.RemoveComponent<CharacteristicsModificationComponent>(entity);
            }).WithReadOnly(parentFilter).WithReadOnly(modificationsBufferFilter).Schedule(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void ApplyModification(Entity entity, Entity holder,
            in CharacteristicsModificationComponent modificationComponent, ComponentDataFromEntity<Parent> parentsFilter,
            BufferFromEntity<CharacteristicModificationsBuffer> modificationsBufferFilter, EntityCommandBuffer ecb)
        {
            if (!parentsFilter.HasComponent(entity))
                return;

            var parent = parentsFilter[entity].Value;

            if (modificationsBufferFilter.HasComponent(parent))
            {
                ecb.AppendToBuffer(parent,
                    new CharacteristicModificationsBuffer()
                        {Value = modificationComponent.Value, ModificatorHolder = holder});
                ecb.AddComponent(parent, new NewCharacteristicsTag());
            }
            else
            {
                ApplyModification(parent, holder, modificationComponent, parentsFilter, modificationsBufferFilter, ecb);
            }
        }
    }
}