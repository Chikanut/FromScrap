using IsVisible.Components;
using Lifetime.Components;
using Unity.Entities;

namespace Lifetime.Systems
{
    public partial class LifetimeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((ref LifetimeComponent lifetime) =>
            {
                lifetime.CurrentLifetime += deltaTime;
            }).ScheduleParallel();
            
            Entities.WithAll<ResetLifeTimeOnVisibleTag>().ForEach((ref LifetimeComponent lifetime, in IsVisibleComponent isVisible) =>
            {
                if (isVisible.Value)
                    lifetime.CurrentLifetime = 0;
            }).ScheduleParallel();
        }
    }
}