using System;
using Reese.Math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Vehicles.Components;
using Vehicles.Wheels.Components;

namespace Vehicles.Wheels.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateBefore(typeof(WheelsSuspensionSystem))]
    public partial class WheelsDriveSystem : SystemBase
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
            var inputInfo = GetComponentDataFromEntity<VehicleInputComponent>(true);
            var world = _createPhysicsWorldSystem.PhysicsWorld;
            var deltaTime = Time.DeltaTime;

            Entities.WithAll<Parent>().ForEach((ref DriveData wheelData, ref Rotation rotation,
                ref Translation translation,
                in LocalToWorld localToWorld) =>
            {
                if (!inputInfo.HasComponent(wheelData.Body)) return;
                
                var bodyTransform = ltw[wheelData.Body];
                var groundInfoData = groundInfoFilter[wheelData.Parent];
                
                var ceIdx = world.GetRigidBodyIndex(wheelData.Body);

                if (-1 == ceIdx || ceIdx >= world.NumDynamicBodies)
                {
                    return;
                }
                
                if (!groundInfoData.isGrounded) return;

                var cePosition = bodyTransform.Position;
                var ceCenterOfMass = world.GetCenterOfMass(ceIdx);
                var ceUp = bodyTransform.Up;

                var wheelPos = groundInfoData.Info.Position;
                wheelPos -= (cePosition - ceCenterOfMass);

                var velocityAtWheel = world.GetLinearVelocity(ceIdx, wheelPos);
                var slopeSlipFactor = math.pow(math.abs(math.dot(ceUp, math.up())), 4.0f);
                var movementInput = (float3)Vector3.ClampMagnitude(inputInfo[wheelData.Body].MoveDir, 1);
                
                var movementPower = math.clamp(math.length(movementInput),0,1);

                if (movementPower <= 0.01f)
                    movementInput = bodyTransform.Forward;

                var dir = wheelData.IsGuide
                    ?  movementInput
                    : (wheelData.IsSubGuide ? (float3)Vector3.Reflect( movementInput, bodyTransform.Right) : bodyTransform.Forward);

                dir = Vector3.RotateTowards(bodyTransform.Forward, dir, math.radians(wheelData.MaxSteerAngle), 100);
                
                
                var weRight = math.cross(groundInfoData.Info.SurfaceNormal,  dir);
                var groundedDir = math.cross(weRight, groundInfoData.Info.SurfaceNormal);
                //Debug.DrawRay(groundInfoData.Info.Position, groundedDir *2, Color.blue);
                
                #region Sideways friction
                {
                    var deltaSpeedRight = (0.0f - (math.dot(velocityAtWheel, weRight)));
                    deltaSpeedRight = math.clamp(deltaSpeedRight, -wheelData.MaxSidewaysImpulse, wheelData.MaxSidewaysImpulse);
                    deltaSpeedRight *= slopeSlipFactor;
                    var impulse = deltaSpeedRight * weRight;
                    var effectiveMass = world.GetEffectiveMass(ceIdx, impulse, wheelPos);
                    impulse *= effectiveMass;
                    
                    world.ApplyImpulse(ceIdx, impulse, groundInfoData.Info.Position);
                }
                
                #endregion
                
                #region Drive
                {
                    if (!wheelData.IsDrive) return;
                    var currentSpeedForward = math.dot(velocityAtWheel, groundedDir);
                    var deltaSpeedForward = (wheelData.MaxSpeed * movementPower - currentSpeedForward);
                    deltaSpeedForward *= wheelData.Acceleration;
                    deltaSpeedForward = math.clamp(deltaSpeedForward, -wheelData.MaxAcceleration, wheelData.MaxAcceleration);
                    deltaSpeedForward *= slopeSlipFactor;
                
                    var driveImpulse = deltaSpeedForward * groundedDir;
                
                    world.ApplyImpulse(ceIdx, driveImpulse, groundInfoData.Info.Position);
                }
                
                #endregion
                
            }).WithReadOnly(inputInfo).WithReadOnly(ltw).WithReadOnly(groundInfoFilter).Schedule();;
        }
    }

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(BuildPhysicsWorld)),
     UpdateBefore(typeof(StepPhysicsWorld))]
    public partial class WheelsSuspensionSystem : SystemBase
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
            var world = _createPhysicsWorldSystem.PhysicsWorld;

            Entities.WithAll<Parent>().ForEach((ref SuspensionData wheelData, ref Translation translation,
                in LocalToWorld localToWorld) =>
            {
                if (!ltw.HasComponent(wheelData.Parent) || !groundInfoFilter.HasComponent(wheelData.Parent)) return;

                var parentTransform = ltw[wheelData.Parent];
                var bodyTransform = ltw[wheelData.Body];
                var groundInfoData = groundInfoFilter[wheelData.Parent];
                
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
                        var lvA = currentSpeedUp * groundInfoData.Info.SurfaceNormal;
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
            }).WithReadOnly(ltw).WithReadOnly(groundInfoFilter).Schedule();
        }
    }

    public partial class WheelsViewSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            var inputInfo = GetComponentDataFromEntity<VehicleInputComponent>(true);
            var ltw = GetComponentDataFromEntity<LocalToWorld>(true);
            var groundInfoFilter = GetComponentDataFromEntity<GroundInfoData>(true);
            
            Entities.WithAll<Parent>().ForEach((ref ViewData wheelData, ref Rotation rotation,
                ref Translation translation,
                in LocalToWorld localToWorld, in DriveData driveData) =>
            {
                if (!ltw.HasComponent(wheelData.Parent) || !groundInfoFilter.HasComponent(wheelData.Parent)) return;
                if (!inputInfo.HasComponent(wheelData.Body)) return;


                var body = ltw[wheelData.Body];
                var input = inputInfo[wheelData.Body];
                var angle = body.Forward.AngleSigned(math.normalizesafe(input.MoveDir), math.up());
                var groundInfo = groundInfoFilter[wheelData.Parent];


                if (wheelData.IsGuide)
                {
                    angle = Mathf.Clamp(angle, -wheelData.TurnRange * 180f, wheelData.TurnRange * 180f);
                    
                    wheelData.SteeringAngle = ECS_Math_Extensions.SmoothDamp(wheelData.SteeringAngle,
                        math.radians(angle), ref wheelData.SteerVelocity, wheelData.TurnDamping, float.MaxValue, deltaTime);

                    rotation.Value = quaternion.AxisAngle(math.up(), wheelData.SteeringAngle);
                }

                var dist = math.distance(localToWorld.Position, wheelData.PrevPosition);
                var moveDir = localToWorld.Position - wheelData.PrevPosition;
                wheelData.PrevPosition = localToWorld.Position;

                UpdateRotation(ref wheelData, ref rotation, localToWorld, body, groundInfo, dist, moveDir);

            }).WithReadOnly(inputInfo).WithReadOnly(groundInfoFilter).WithReadOnly(ltw).ScheduleParallel();
        }
        
        private static void UpdateRotation(ref ViewData viewData, ref Rotation rotation, LocalToWorld localToWorld,
            LocalToWorld parentTransform,
            GroundInfoData groundInfoData, float moveDist, float3 moveDir)
        {
            if (groundInfoData.isGrounded)
            {
                var targetAngle = moveDist / viewData.Radius;
        
                targetAngle *= math.sign(math.dot(localToWorld.Right, parentTransform.Right));
                targetAngle *= math.sign(math.dot(moveDir, parentTransform.Forward));
        
                if (viewData.IsGuide)
                {
                    viewData.CurrentAngle += targetAngle;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(viewData.CurrentAngle));
                }
                else
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }
            }
            else
            {
                var targetAngle = 0.01f / viewData.Radius;
                targetAngle *= math.sign(math.dot(localToWorld.Right, parentTransform.Right));
        
                if (viewData.IsGuide)
                {
                    viewData.CurrentAngle += targetAngle;
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(viewData.CurrentAngle));
                }
                else
                {
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateX(targetAngle));
                }
            }
        }
    }
}