using DamageSystem.Components;
using DamageSystem.Systems;
using Ram.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;

namespace Ram.Systems
{
    [UpdateAfter(typeof(DamageCollisionSystem))]
    public partial class RamSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var velocityFilter = GetComponentDataFromEntity<PhysicsVelocity>();
            
            Entities.WithAll<PhysicsVelocity>().ForEach((Entity entity, ref DealDamage dealDamage, in RamComponent ramComponent, in DynamicBuffer<StatefulCollisionEvent> collisionEvents) =>
            {
                if(dealDamage.isReloading) return;
                
                var velocity = velocityFilter[entity];
                var speed = velocity.Linear.Magnitude();

                if (speed < ramComponent.SpeedRange.x)
                {
                    dealDamage.Value = 0;
                    return;
                }
                
                var power = (speed - ramComponent.SpeedRange.x) / (ramComponent.SpeedRange.y - ramComponent.SpeedRange.x);
                power = math.clamp(power, 0, 1);

                var impulse = math.lerp(ramComponent.ImpulseRange.x, ramComponent.ImpulseRange.y, power);
                var damage = math.lerp(ramComponent.DamageRange.x, ramComponent.DamageRange.y, power);

                dealDamage.Value = (int)damage;

                for (int i = 0; i < collisionEvents.Length; i++)
                {
                    var collidedBody = collisionEvents[i].GetOtherEntity(entity);
                    
                    if(!velocityFilter.HasComponent(collidedBody)) continue;

                    var collidedVelocity = velocityFilter[collidedBody];
                    
                    collidedVelocity.Linear += impulse * collisionEvents[i].GetNormalFrom(entity);
                    
                    velocityFilter[collidedBody] = collidedVelocity;
                }
            }).Schedule();
        }
    }
}