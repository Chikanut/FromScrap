using System;
using DamageSystem.Components;
using Magnet.Components;
using MoveTo.Components;
using StatisticsSystem.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Stateful;

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

    public partial class MagnetRescaleSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref SphereTriggerComponent sphereTriggerComponent, in CharacteristicsComponent characteristic, in MagnetComponent magnetComponent) =>
            {
                if(Math.Abs(magnetComponent.Radius * characteristic.Value.AreaMultiplier - sphereTriggerComponent.Radius) > 0.05f)
                    sphereTriggerComponent.Radius = magnetComponent.Radius * characteristic.Value.AreaMultiplier;
                
            }).ScheduleParallel();
        }
    }
}