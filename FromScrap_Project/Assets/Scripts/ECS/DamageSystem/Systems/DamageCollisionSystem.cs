using DamageSystem.Components;
using Unity.Entities;
using Unity.Physics.Stateful;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(ResolveDamageSystem))]
    public partial class DamageCollisionSystem : SystemBase
    {
        protected override void OnUpdate()
        {                                                        
            var damageBuffers = GetBufferFromEntity<Damage>();
            var time = Time.ElapsedTime;
            
            Entities.WithNone<Dead>().ForEach((Entity entity, ref DealDamage dealDamage, in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
            {
                if(dealDamage.isReloading) return;
                
                foreach (var triggerEvent in triggerEvents)
                {
                    if(triggerEvent.State != EventOverlapState.Enter) continue;
                    
                    dealDamage.CurrentHit++;
                    
                    var otherEntity = triggerEvent.GetOtherEntity(entity);

                    if (!damageBuffers.HasComponent(otherEntity))
                    {
                        dealDamage.CurrentHit = dealDamage.MaxHits;
                        continue;
                    }
                    
                    damageBuffers[otherEntity].Add(new Damage() {Value = dealDamage.Value});
                    dealDamage.PrevHitTime = time;
                }
            }).Schedule();
            
            Entities.WithNone<Dead>().ForEach((Entity entity, ref DealDamage dealDamage, in DynamicBuffer<StatefulCollisionEvent> collisionEvents) =>
            {
                if(dealDamage.isReloading) return;
                
                foreach (var collisionEvent in collisionEvents)
                {
                    if(collisionEvent.CollidingState != EventCollidingState.Enter) continue;
                    
                    dealDamage.CurrentHit++;
                    
                    var otherEntity = collisionEvent.GetOtherEntity(entity);

                    if (!damageBuffers.HasComponent(otherEntity))
                    {
                        dealDamage.CurrentHit = dealDamage.MaxHits;
                        continue;
                    }
                    
                    damageBuffers[otherEntity].Add(new Damage() {Value = dealDamage.Value});
                    dealDamage.PrevHitTime = time;
                }
            }).Schedule();
        }
    }
}