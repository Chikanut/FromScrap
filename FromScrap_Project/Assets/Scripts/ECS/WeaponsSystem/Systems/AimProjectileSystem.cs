using ECS.FindTargetSystem;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    public partial class AimProjectileSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<AimProjectileTag>().ForEach((Entity entity, ref ShotTemporaryData tempData, in HasTarget hasTarget, in LocalToWorld localToWorld) =>
            {
                tempData.Direction = math.normalizesafe(hasTarget.TargetPosition - localToWorld.Position);
            }).ScheduleParallel();
        }
    }
}