using DamageSystem.Components;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(SpawnOnDeathSystem))]
    public partial class DestroyOnContactSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        private StepPhysicsWorld _stepPhysicsWorld;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();

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
            
            Entities.WithNone<Dead>().WithAll<DestroyOnContact>().ForEach((Entity entity, in DynamicBuffer<StatefulCollisionEvent> collisionEvents) =>
            {
                for (int i = 0; i < collisionEvents.Length; i++)
                {
                    if (collisionEvents[i].CollidingState != EventCollidingState.Enter) continue;
                    ecb.AddComponent(entity, new Dead());
                    break;
                }
            }).Schedule();
        }
    }
}