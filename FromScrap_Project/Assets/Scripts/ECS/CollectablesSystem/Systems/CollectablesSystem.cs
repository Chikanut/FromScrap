using Collectables.Components;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

namespace Collectables.Systems
{
    public partial class CollectablesSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var playerFilter = GetComponentDataFromEntity<PlayerTag>(true);
            var ecb = _ecbSystem.CreateCommandBuffer();

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, in CollectableTriggerComponent collectable, in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
            {
                foreach (var triggerEvent in triggerEvents)
                {
                    var otherEntity = triggerEvent.GetOtherEntity(entity);
                    
                    if (!playerFilter.HasComponent(otherEntity))
                        continue;

                    ecb.AddComponent(collectable.MainObject,
                        new CollectableGatheredComponent() {CollectedEntity = otherEntity});

                    break;

                }
            }).WithReadOnly(playerFilter).Schedule(Dependency);

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}