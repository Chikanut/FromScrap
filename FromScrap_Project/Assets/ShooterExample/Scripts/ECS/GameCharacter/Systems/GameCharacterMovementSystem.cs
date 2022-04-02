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
            ) => {

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
            float boost = movementComponent.BoostKey ? movementComponent.BoostSpeedMultiplier : 1.0f;
            
            movementComponent.CurrentVelocity = Vector3.Lerp(
                movementComponent.CurrentVelocity,
                newDirection * movementComponent.MaxSpeed * boost * fixedDeltaTime, 
                movementComponent.MaxAcceleration * fixedDeltaTime);

            velocity.Linear = new float3(movementComponent.CurrentVelocity.x, velocity.Linear.y, movementComponent.CurrentVelocity.z);

            var dir = Vector3.Normalize(velocity.Linear);
            dir.y = 0f;
            
            if(math.abs(movementComponent.VerticalAxis) + math.abs(movementComponent.HorizontalAxis) > 0)
                movementComponent.CurrentDirection = Vector3.Lerp(
                    movementComponent.CurrentDirection,
                    dir, movementComponent.RotationSpeed * deltaTime);
            
            rotation.Value = math.slerp(
                rotation.Value, 
                quaternion.LookRotationSafe(movementComponent.CurrentDirection, new float3(0, 1, 0)), 
                movementComponent.RotationSpeed * deltaTime
                );
        }).ScheduleParallel();
    }
}
