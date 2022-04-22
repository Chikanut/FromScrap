using DamageSystem.Components;
using DamageSystem.Systems;
using Magnet.Components;
using MoveTo.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;

namespace Magnet.Systems
{
    [UpdateInGroup(typeof (SimulationSystemGroup), OrderFirst = true), UpdateBefore(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial class MagnetSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem _ecbSystem;
        private JobHandle _bufferSetJobHandle;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {

            var ecb = _ecbSystem.CreateCommandBuffer();
            var moveTo = GetComponentDataFromEntity<MoveToComponent>(true);
            var dead = GetComponentDataFromEntity<Dead>(true);
            
            Dependency = Entities.ForEach((Entity entity, in MagnetComponent magnetComponent,
                in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
            {
                for (int i = 0; i < triggerEvents.Length; i++)
                {
                    var otherEntity = triggerEvents[i].GetOtherEntity(entity);
                    if (!dead.HasComponent(otherEntity) && !moveTo.HasComponent(otherEntity))
                    {
                        ecb.AddComponent(otherEntity,
                            new MoveToComponent() {Speed = magnetComponent.Speed, Target = entity});
                    }
                }
            }).WithReadOnly(dead).WithReadOnly(moveTo).Schedule(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);

        }
    }
}