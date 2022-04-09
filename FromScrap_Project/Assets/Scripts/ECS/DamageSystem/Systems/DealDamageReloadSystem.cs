using DamageSystem.Components;
using Unity.Entities;

namespace DamageSystem.Systems
{
    public partial class DealDamageReloadSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = Time.ElapsedTime;
            
            Entities.ForEach((ref DealDamage dealDamage) =>
            {
                dealDamage.isReloading = time - dealDamage.PrevHitTime < dealDamage.DamageDelay;
            }).ScheduleParallel();
        }
    }
}