using Cars.View.Components;
using Reese.Math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Vehicles.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(BuildPhysicsWorld)),
     UpdateBefore(typeof(StepPhysicsWorld))]
    public partial class WheelsSystem : SystemBase
    {
        BuildPhysicsWorld _createPhysicsWorldSystem;

        protected override void OnCreate()
        {
            _createPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }
        
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            this.RegisterPhysicsRuntimeSystemReadWrite();
        }

        protected override void OnUpdate()
        {
            var ltw = GetComponentDataFromEntity<LocalToWorld>(true);
            var groundInfoFilter = GetComponentDataFromEntity<GroundInfoData>(true);
            var deltaTime = Time.DeltaTime;
            var world = _createPhysicsWorldSystem.PhysicsWorld;

            Entities.WithAll<Parent>().ForEach((ref WheelData wheelData, ref Rotation rotation,
                ref Translation translation,
                in LocalToWorld localToWorld) =>
            {
                if (!ltw.HasComponent(wheelData.Parent) || !groundInfoFilter.HasComponent(wheelData.Parent)) return;
                
                var parentTransform = ltw[wheelData.Parent];
                var bodyTransform = ltw[wheelData.Body];
                var groundInfoData = groundInfoFilter[wheelData.Parent];
                
                var dist = math.distance(localToWorld.Position, wheelData.PrevPos);
                var moveDir = localToWorld.Position - wheelData.PrevPos;
                wheelData.PrevPos = localToWorld.Position;

                if (wheelData.isGuide)
                    UpdateSteering(ref wheelData, ref rotation, moveDir, parentTransform, deltaTime);
                UpdateRotation(ref wheelData, ref rotation, localToWorld, parentTransform, groundInfoData, dist,
                    moveDir);

                #region UpdateSuspension

                var ceIdx = world.GetRigidBodyIndex(wheelData.Body);
                
                if (-1 == ceIdx || ceIdx >= world.NumDynamicBodies)
                {
                    return;
                }

                var cePosition = bodyTransform.Position;
                var ceCenterOfMass = world.GetCenterOfMass(ceIdx);
                var ceUp = bodyTransform.Up;

                var rayStart = parentTransform.Position;
                var rayEnd = (-ceUp * (wheelData.SuspensionDistance + wheelData.Radius)) + rayStart;

                var wheelPos = groundInfoData.Info.Position;
                wheelPos -= (cePosition - ceCenterOfMass);

                var velocityAtWheel = world.GetLinearVelocity(ceIdx, wheelPos);

                var currentSpeedUp = math.dot(velocityAtWheel, ceUp);
                var wheelCurrentPos = localToWorld.Position;
                var hit = groundInfoData.isGrounded;

                if (!hit)
                {
                    var wheelDesiredPos = (-ceUp * wheelData.SuspensionDistance) + rayStart;
                    translation.Value = math.lerp(wheelCurrentPos, wheelDesiredPos,
                            wheelData.SuspensionDamping / wheelData.SuspensionStrength)
                        .ToLocal(parentTransform);
                }
                else
                {
                    // remove the wheelbase to get wheel position.
                    var fraction = groundInfoData.Info.Fraction - (wheelData.Radius) /
                        (wheelData.SuspensionDistance + wheelData.Radius);

                    var wheelDesiredPos = math.lerp(rayStart, rayEnd, fraction);
                    translation.Value = math.lerp(wheelCurrentPos, wheelDesiredPos,
                            wheelData.SuspensionDamping / wheelData.SuspensionStrength)
                        .ToLocal(parentTransform);

                    #region Suspension

                    {
                        // Calculate and apply the impulses
                        var posA = rayEnd;
                        var posB = groundInfoData.Info.Position;
                        var lvA = currentSpeedUp * ceUp;
                        var lvB = world.GetLinearVelocity(groundInfoData.Info.RigidBodyIndex, posB);

                        var impulse = wheelData.SuspensionStrength * (posB - posA) +
                                      wheelData.SuspensionDamping * (lvB - lvA);
   
                        var impulseUp = math.dot(impulse, ceUp);

                        // Suspension shouldn't necessarily pull the vehicle down!
                        var downForceLimit = -0.25f;
                        if (downForceLimit < impulseUp)
                        {
                            impulse = impulseUp * ceUp;

                            world.ApplyImpulse(ceIdx, impulse, posA);
                        }
                    }

                    #endregion
                }

                #endregion
                
            }).WithReadOnly(ltw).WithReadOnly(groundInfoFilter).Schedule();
        }

        private static void UpdateSteering(ref WheelData wheelData, ref Rotation rotation, float3 moveDir,
            LocalToWorld parentTransform, float deltaTime)
        {
            moveDir.y = 0;

            var power = moveDir.Magnitude();

            if (power > 0.05f)
            {
                moveDir = math.normalize(moveDir);

                var input = math.dot(parentTransform.Right, moveDir);

                if (!float.IsNaN(input))
                {
                    var targetDir = math.lerp(math.forward(), math.right() * math.sign(input),
                        wheelData.TurnRange * math.abs(input));

                    wheelData.TurnDirection = ECS_Math_Extensions.SmoothDamp(wheelData.TurnDirection, targetDir,
                        ref wheelData.TurnVelocity, wheelData.TurnDamping, float.MaxValue, deltaTime);
                }
            }

            rotation.Value =
                quaternion.LookRotationSafe(wheelData.TurnDirection, math.up());
        }

        private static void UpdateRotation(ref WheelData wheelData, ref Rotation rotation, LocalToWorld localToWorld,
            LocalToWorld parentTransform,
            GroundInfoData groundInfoData, float moveDist, float3 moveDir)
        {
            if (groundInfoData.isGrounded)
            {
                var targetAngle = moveDist / wheelData.Radius;

                targetAngle *= math.sign(math.dot(localToWorld.Right, parentTransform.Right));
                targetAngle *= math.sign(math.dot(moveDir, parentTransform.Forward));

                if (wheelData.isGuide)
                {
                    wheelData.CurrentAngle += targetAngle;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(wheelData.CurrentAngle));
                }
                else
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }
            }
            else
            {
                var targetAngle = 0.01f / wheelData.Radius;
                targetAngle *= math.sign(math.dot(localToWorld.Right, parentTransform.Right));

                if (wheelData.isGuide)
                {
                    wheelData.CurrentAngle += targetAngle;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(wheelData.CurrentAngle));
                }
                else
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }
            }
        }
    }
}