using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;
using ECS.SignalSystems.Systems;
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
          
            
            Dependency = Entities.ForEach((Entity entity, ref CharacteristicsModificationComponent modificationComponent) =>
            {
                if (ApplyModification(entity, entity, modificationComponent, parentFilter, modificationsBufferFilter,
                        ecb) || modificationComponent.CurrentTryUpdateTimes >= CharacteristicsModificationComponent.MaxTryUpdateTimes)
                {
                    ecb.RemoveComponent<CharacteristicsModificationComponent>(entity);
                }
                else
                {
                    modificationComponent.CurrentTryUpdateTimes++;
                }
                
            }).WithReadOnly(parentFilter).WithReadOnly(modificationsBufferFilter).Schedule(Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static bool ApplyModification(Entity entity, Entity holder,
            in CharacteristicsModificationComponent modificationComponent, ComponentDataFromEntity<Parent> parentsFilter,
            BufferFromEntity<CharacteristicModificationsBuffer> modificationsBufferFilter, EntityCommandBuffer ecb)
        {
            if (!parentsFilter.HasComponent(entity))
            {
                return false;
            }

            var parent = parentsFilter[entity].Value;

            if (!modificationsBufferFilter.HasComponent(parent))
                return ApplyModification(parent, holder, modificationComponent, parentsFilter,
                    modificationsBufferFilter, ecb);
            
            ecb.AppendToBuffer(parent,
                new CharacteristicModificationsBuffer()
                    {Value = modificationComponent.Value, ModificatorHolder = holder, Multiply = modificationComponent.Multiply});
         
            ecb.AddComponent(parent, new NewCharacteristicsTag());

            return true;
        }
    }
}