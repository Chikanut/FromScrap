using DamageSystem.Components;
using Unity.Entities;
using Unity.Physics.Stateful;

namespace DamageSystem.Systems
{
    [UpdateAfter(typeof(ResolveDamageSystem))]
    public partial class DestroyOnContactSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();

            Entities.WithNone<Dead>().ForEach((Entity entity, in DestroyOnContact destroyOnContact, in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
            {
                for (int i = 0; i < triggerEvents.Length; i++)
                {
                    if (triggerEvents[i].State != EventOverlapState.Enter ||
                        !destroyOnContact.IncludeTriggerEvent) continue;
                    ecb.AddComponent(entity, new Dead());
                    break;
                }

            }).Schedule();
            
            Entities.WithNone<Dead>().ForEach((Entity entity, in DestroyOnContact destroyOnContact, in DynamicBuffer<StatefulCollisionEvent> collisionEvents) =>
            {
                for (int i = 0; i < collisionEvents.Length; i++)
                {
                    if (collisionEvents[i].CollidingState != EventCollidingState.Enter ||
                        !destroyOnContact.IncludeCollisionEvents) continue;
                    ecb.AddComponent(entity, new Dead());
                    break;
                }
            }).Schedule();
        }
    }
}