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

            var longitudinalNormal = new Vector3(0f, 1f, 0f);
            var lateralNormal = new Vector3(0f, 0f, 1f);
            var horizonLevel = Vector3.Dot(localToWorld.Right, longitudinalNormal);
            var longitudinalStabilityLevel = Vector3.Dot(localToWorld.Up, longitudinalNormal);
            var lateralStabilityLevel = Vector3.Dot(localToWorld.Forward, lateralNormal);
            //
            // if (horizonLevel < movementComponent.CarCriticalMovementLevel)
            //     newDirection = float3.zero;

            if (carWheelContactedCount > 0)
            {
                var boost = movementComponent.BoostKey ? movementComponent.BoostSpeedMultiplier : 1.0f;
                var carStabilizationSpeedMultiplier = 0.5f * (longitudinalStabilityLevel + 1) / 2;

                if (!movementComponent.IsLongitudinalStabilization)
                    carStabilizationSpeedMultiplier = 1f;

                movementComponent.CurrentSpeedModificator = carStabilizationSpeedMultiplier * (carWheelContactedCount / 4f * 0.5f + 0.5f);

                movementComponent.CurrentVelocity = Vector3.Lerp(
                    movementComponent.CurrentVelocity,
                    newDirection * movementComponent.MaxSpeed * boost * movementComponent.CurrentSpeedModificator *
                    fixedDeltaTime,
                    movementComponent.MaxAcceleration * fixedDeltaTime);

                velocity.Linear = new float3(
                    movementComponent.CurrentVelocity.x,
                    velocity.Linear.y,
                    movementComponent.CurrentVelocity.z);

                if (carWheelContactedCount > 1)
                {
                    var dir = new float3(0f, 0f, 0f);

                    if (Vector3.Magnitude(newDirection) > 0)
                        dir = Vector3.Normalize(velocity.Linear);

                    dir.y = 0f;

                    var orientTarget = localToWorld.Right;
                    var orient = Vector3.Dot(dir, orientTarget);

                    orient = Mathf.Clamp(orient, -1f, 1f);
                    movementComponent.CurrentTurnSpeedModificator = carWheelContactedCount / 4f;

                    if (math.abs(movementComponent.VerticalAxis) + math.abs(movementComponent.HorizontalAxis) > 0)
                        movementComponent.CurrentDirection = Vector3.Lerp(
                            movementComponent.CurrentDirection,
                            dir,
                            movementComponent.TurnSpeed * movementComponent.CurrentTurnSpeedModificator * deltaTime);

                    velocity.Angular.y = orient * movementComponent.TurnSpeed;
                }
            }
            
            var alpha = horizonLevel < 0? 1: -1;
            var carStabilizationForceMultiplier = (1 - carWheelContactedCount / 4) * 0.7f + 0.3f;

            if (longitudinalStabilityLevel < movementComponent.CarLongitudinalStabilizationStartLevel && carWheelContactedCount == 0)
                movementComponent.IsLongitudinalStabilization = true;

            if (longitudinalStabilityLevel > movementComponent.CarLongitudinalStabilizationEndLevel)
                movementComponent.IsLongitudinalStabilization = false;

            if (movementComponent.IsLongitudinalStabilization && Vector3.Magnitude(newDirection) > 0)
                velocity.Angular.z = alpha * movementComponent.CarLongitudinalStabilizationForce * fixedDeltaTime * carStabilizationForceMultiplier;

            if (lateralStabilityLevel < movementComponent.CarLateralStabilizationStartLevel && carWheelContactedCount == 0)
                movementComponent.IsLateralStabilization = true;

            if (lateralStabilityLevel > movementComponent.CarLateralStabilizationEndLevel)
                movementComponent.IsLateralStabilization = false;

            if (movementComponent.IsLateralStabilization && Vector3.Magnitude(newDirection) > 0)
                velocity.Angular.x = -alpha * movementComponent.CarLateralStabilizationForce * fixedDeltaTime * carStabilizationForceMultiplier;
           
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