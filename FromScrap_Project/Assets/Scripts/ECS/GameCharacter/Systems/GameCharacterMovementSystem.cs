using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public partial class GameCharacterMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        float deltaTime = Time.DeltaTime;

        var groundInfo = GetComponentDataFromEntity<GroundInfoData>(true);
        var multyGroundInfo = GetBufferFromEntity<MultyGroundInfoData>(true);

        Entities.WithAll<
            GameCharacterMovementComponent>().ForEach((Entity entity,
            ref GameCharacterMovementComponent movementComponent,
            ref PhysicsVelocity velocity,
            ref PhysicsMass mass,
            ref LocalToWorld localToWorld,
            ref Rotation rotation 
            //ref PhysicsJoint PJ
        ) =>
        {
            if (movementComponent.SpaceKey
                //|| movementComponent.IsStabilization
                )
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

            /*
            if (groundInfo.HasComponent(entity) && !groundInfo[entity].isGrounded)
            {
                return;
            }
            */

            var carWheelContactedCount = 0;
            
            if (multyGroundInfo.HasComponent(entity))
            {
                var groundInfos = multyGroundInfo[entity];

                foreach (var groundInfoData in groundInfos)
                    if (groundInfoData.isGrounded)
                        carWheelContactedCount += 1;

                //if (!collided && !movementComponent.SpaceKey)
                //    return;
            }

            var horizonNormal = new Vector3(0f, 1f, 0f);
            var horizonLevel = Vector3.Dot(localToWorld.Right, horizonNormal);
            var horizonStabilityLevel = Vector3.Dot(localToWorld.Up, horizonNormal);
            //
            // if (horizonLevel < movementComponent.CarCriticalMovementLevel)
            //     newDirection = float3.zero;

            if (carWheelContactedCount > 0)
            {
                float boost = movementComponent.BoostKey ? movementComponent.BoostSpeedMultiplier : 1.0f;

                movementComponent.CurrentSpeedModificator = carWheelContactedCount / 4f * 0.5f + 0.5f;

                movementComponent.CurrentVelocity = Vector3.Lerp(
                    movementComponent.CurrentVelocity,
                    newDirection * movementComponent.MaxSpeed * boost * movementComponent.CurrentSpeedModificator *
                    fixedDeltaTime,
                    movementComponent.MaxAcceleration * fixedDeltaTime);

                velocity.Linear = new float3(
                    movementComponent.CurrentVelocity.x,
                    velocity.Linear.y,
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
            }

            if (horizonStabilityLevel < movementComponent.CarStabilizationStartLevel && carWheelContactedCount == 0)
                movementComponent.IsStabilization = true;

            if (horizonStabilityLevel > movementComponent.CarStabilizationEndLevel)
            {
                movementComponent.IsStabilization = false;
                movementComponent.CurrentSpeedModificator = 1f;
            }

            if (movementComponent.IsStabilization && Vector3.Magnitude(newDirection) > 0) 
            {
                var alpha = horizonLevel < 0? 1: -1;
                
                movementComponent.CurrentSpeedModificator = (horizonStabilityLevel + 1) / 2;
                velocity.Angular.z = alpha * movementComponent.CarStabilizationSpeed * fixedDeltaTime;// * Mathf.Abs(horizonLevel);
                velocity.Angular.x = -alpha * movementComponent.CarStabilizationSpeed * fixedDeltaTime;// * Mathf.Abs(horizonLevel);
            }
            /*
            var angular = new float3(0f, 2f, 0f);
            var q = rotation.Value;
            var angle = 0.0001f * math.atan(q.value.y / q.value.w);
            rotation.Value = quaternion.RotateY(angle);
            velocity.SetAngularVelocityWorldSpace(mass, rotation, angular);
            */
            /*
            var constraints = PJ.GetConstraints();
 
            var c0 = constraints[0];
            c0.Min = 0;
            c0.Max = 5;
            constraints[0] = c0;
            //PJ.SetConstraints(constraints);
            */
        }).
            //WithReadOnly(groundInfo).
            WithReadOnly(multyGroundInfo).
            ScheduleParallel();
    }
}