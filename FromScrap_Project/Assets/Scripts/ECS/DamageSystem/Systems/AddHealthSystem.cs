using DamageSystem.Components;
using LevelingSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using UnityEngine.Rendering;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(ResolveDamageSystem))]
    public partial class AddHealthSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var health = GetComponentDataFromEntity<Health>();
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, in AddHealthComponent addHealth, in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
            {
                foreach (var triggerEvent in triggerEvents)
                {
                    // if(triggerEvent.State != EventOverlapState.Enter) continue;

                    var otherEntity = triggerEvent.GetOtherEntity(entity);
                   
                    if (!health.HasComponent(otherEntity)) continue;

                    var h = health[otherEntity];
                    // h.Value = math.clamp( h.Value + addHealth.Value, 0, h.InitialValue);
                    h.AddHealth(addHealth.Value);
                    health[otherEntity] = h;
                    
                    ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                   
                    break;

                }
            }).Schedule(Dependency);

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}