using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial class GameCharacterMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<
            GameCharacterMovementComponent>().ForEach((
            ref GameCharacterMovementComponent movementComponent,
            ref PhysicsVelocity velocity,
            ref PhysicsMass mass,
            ref LocalToWorld localToWorld,
            ref Rotation rotation
        ) =>
        {

            if (movementComponent.SpaceKey)
            {
                movementComponent.CurrentJetPower = math.lerp(
                    movementComponent.CurrentJetPower,
                    movementComponent.JetPower * fixedDeltaTime,
                    movementComponent.JetPowerAcceleration * fixedDeltaTime);
                velocity.Linear.y = movementComponent.CurrentJetPower;
            }
            else
            {
                movementComponent.CurrentJetPower = 0f;
            }

            var newDirection = new float3(movementComponent.HorizontalAxis, 0f, movementComponent.VerticalAxis);
            newDirection = Vector3.ClampMagnitude(newDirection, 1f);

            // var horizonNormal = new Vector3(0f, 1f, 0f);
            // var horizonLevel = Vector3.Dot(localToWorld.Up, horizonNormal);
            //
            // if (horizonLevel < movementComponent.CarCriticalMovementLevel)
            //     newDirection = float3.zero;

            float boost = movementComponent.BoostKey ? movementComponent.BoostSpeedMultiplier : 1.0f;

            movementComponent.CurrentVelocity = Vector3.Lerp(
                movementComponent.CurrentVelocity,
                newDirection * movementComponent.MaxSpeed * boost * fixedDeltaTime,
                movementComponent.MaxAcceleration * fixedDeltaTime);

            velocity.Linear = new float3(movementComponent.CurrentVelocity.x, velocity.Linear.y,
                movementComponent.CurrentVelocity.z);

            float3 dir = Vector3.Normalize(velocity.Linear);
            dir.y = 0f;

            var orient = Vector3.Dot(dir, localToWorld.Right);

            if (math.abs(movementComponent.VerticalAxis) + math.abs(movementComponent.HorizontalAxis) > 0)
                movementComponent.CurrentDirection = Vector3.Lerp(
                    movementComponent.CurrentDirection,
                    dir,
                    movementComponent.RotationSpeed * deltaTime);

            velocity.Angular.y = orient * movementComponent.RotationSpeed;

            // if (horizonLevel < movementComponent.CarStabilizationLevel)
            // {
            //     velocity.Angular.x = (1f - horizonLevel) * movementComponent.CarStabilizationSpeed * fixedDeltaTime;
            //     velocity.Angular.z = (1f - horizonLevel) * movementComponent.CarStabilizationSpeed * fixedDeltaTime;
            // }
        }).ScheduleParallel();
    }
}
