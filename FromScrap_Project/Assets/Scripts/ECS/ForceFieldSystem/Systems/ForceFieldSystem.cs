using ForceField.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace ForceField.Systems
{
    public partial class ForceFieldSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbParallel = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
            var addForceFilter = GetComponentDataFromEntity<AddForceComponent>(true);
            var localToWorldFilter = GetComponentDataFromEntity<LocalToWorld>(true);

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, DynamicBuffer<StatefulTriggerEvent> triggerEvents, ref ForceFieldComponent fieldComponent, in LocalToWorld localToWorld) =>
            {
                foreach (var triggerEvent in triggerEvents)
                {
                    var other = triggerEvent.GetOtherEntity(entity);
                    if(addForceFilter.HasComponent(other) || !localToWorldFilter.HasComponent(other)) continue;
                    
                    ecbParallel.AddComponent(other, new AddForceComponent()
                    {
                        Dir = math.normalizesafe(localToWorldFilter[other].Position - localToWorld.Position) * (fieldComponent.ForceIn ? -1 : 1),
                        Force = fieldComponent.Force,
                    });
                    
                }
            }).WithReadOnly(addForceFilter).WithReadOnly(localToWorldFilter).Schedule(Dependency);
            
            _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}