using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    public partial class MoveShotSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((Entity e, ref Translation translation, ref Rotation rotation,
                ref ShotTemporaryData tempData, in ShotData shotData, in LocalToWorld localToWorld) =>
            {
                if (!tempData.MoveShot) return;

                tempData.CurrentDirection = ECS_Math_Extensions.SmoothDamp(tempData.CurrentDirection,
                    tempData.Direction,
                    ref tempData.DirVelocity, shotData.DirectionDamping, float.MaxValue, deltaTime);

                tempData.CurrentSpeed = ECS_Math_Extensions.SmoothDamp(tempData.CurrentSpeed,
                    shotData.Velocity,
                    ref tempData.SpeedVelocity, shotData.SpeedDamping, float.MaxValue, deltaTime);

                var velocity = tempData.CurrentDirection * tempData.CurrentSpeed *
                               tempData.Characteristics.ProjectileSpeedMultiplier;

                var x = velocity.x * deltaTime;
                var z = velocity.z * deltaTime;
                var y = velocity.y * deltaTime - shotData.Gravity * math.pow(deltaTime, 2) / 2;

                var prevPos = translation.Value;
                translation.Value += new float3(x, y, z);

                var dir = math.normalize(translation.Value - prevPos);

                rotation.Value = quaternion.LookRotation(dir, math.up());

            }).ScheduleParallel();
        }
    }
}