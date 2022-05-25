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
            var damageBlockers = GetComponentDataFromEntity<DamageBlockTimer>(true);
            var time = Time.ElapsedTime;
            
            Entities.WithNone<Dead>().ForEach((Entity entity, ref DealDamage dealDamage, in DynamicBuffer<StatefulTriggerEvent> triggerEvents) =>
            {
                if(dealDamage.isReloading || dealDamage.Value <= 0) return;
                
                foreach (var triggerEvent in triggerEvents)
                {
                    dealDamage.CurrentHit++;
                    
                    var otherEntity = triggerEvent.GetOtherEntity(entity);

                    if (!damageBuffers.HasComponent(otherEntity))
                    {
                        dealDamage.CurrentHit = dealDamage.MaxHits;
                        continue;
                    }
                    
                    if(!damageBlockers.HasComponent(otherEntity))
                        damageBuffers[otherEntity].Add(new Damage() {Value = dealDamage.Value, isPlayer = dealDamage.isPlayer});
                    
                    dealDamage.PrevHitTime = time;
                }
            }).WithReadOnly(damageBlockers).Schedule();
            
            Entities.WithNone<Dead>().ForEach((Entity entity, ref DealDamage dealDamage, in DynamicBuffer<StatefulCollisionEvent> collisionEvents) =>
            {
                if(dealDamage.isReloading || dealDamage.Value <= 0) return;
                
                foreach (var collisionEvent in collisionEvents)
                {
                    dealDamage.CurrentHit++;
                    
                    var otherEntity = collisionEvent.GetOtherEntity(entity);

                    if (!damageBuffers.HasComponent(otherEntity))
                    {
                        dealDamage.CurrentHit = dealDamage.MaxHits;
                        continue;
                    }
                    
                    if(!damageBlockers.HasComponent(otherEntity))
                        damageBuffers[otherEntity].Add(new Damage() {Value = dealDamage.Value, isPlayer = dealDamage.isPlayer});
                    
                    dealDamage.PrevHitTime = time;
                }
            }).WithReadOnly(damageBlockers).Schedule();
        }
    }
}