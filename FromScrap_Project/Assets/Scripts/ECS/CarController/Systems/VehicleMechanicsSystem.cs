using Reese.Math;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Vehicles.Components;
using RaycastHit = Unity.Physics.RaycastHit;
using VertexFragment;

namespace Vehicles.Systems
{

    #region System
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(StepPhysicsWorld))]
    public partial class VehicleMechanicsSystem : SystemBase
    {
        BuildPhysicsWorld _createPhysicsWorldSystem;
        EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _createPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            this.RegisterPhysicsRuntimeSystemReadWrite();
        }


        // Update is called once per frame
        protected override void OnUpdate()
        {
            var world = _createPhysicsWorldSystem.PhysicsWorld;

            var localToWorldFilter = GetComponentDataFromEntity<LocalToWorld>(true);
            var rotationFilter = GetComponentDataFromEntity<Rotation>(true);
            var translationFilter = GetComponentDataFromEntity<Translation>(true);
            var parentFilter = GetComponentDataFromEntity<Parent>(true);
            var childFilter = GetBufferFromEntity<Child>(true);
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Entities.ForEach((Entity entity, in Translation translation, in Rotation rotation, in VehicleSettingsComponent mechanics, in DynamicBuffer<WheelsBuffer> wheelsBuffer, in LocalToWorld localToWorld) =>
                {
                    if (wheelsBuffer.Length == 0) return;

                    if (entity == Entity.Null)
                    {
                        Debug.LogError("NULL");
                        return;
                    }

                    var ceIdx = world.GetRigidBodyIndex(entity);
                    if (-1 == ceIdx || ceIdx >= world.NumDynamicBodies)
                    {
                        Debug.LogError("NULL");
                        return;
                    }
                    
                    var cePosition = translation.Value;
                    var ceCenterOfMass = world.GetCenterOfMass(ceIdx);

                    var ceUp = localToWorld.Up;
                    var ceForward = localToWorld.Forward;
                    var ceRight = localToWorld.Right;

                    var rayResults = new NativeArray<RaycastHit>(wheelsBuffer.Length, Allocator.Temp);

                    var filter = mechanics.CollisionFilter;
                    for (var i = 0; i < wheelsBuffer.Length; i++)
                    {
                        var weEn = wheelsBuffer[i].Wheel;
                    
                        if (!parentFilter.HasComponent(weEn)) continue;
                    
                        var rayStart = localToWorldFilter[parentFilter[weEn].Value].Position;
                        var rayEnd = (-ceUp * (mechanics.SuspensionLength + mechanics.WheelBase)) + rayStart;
                        #if UNITY_EDITOR
                        if (mechanics.DrawDebugInformation)
                            Debug.DrawRay(rayStart, rayEnd - rayStart);
                        #endif
                    
                        var (isHit, hitInfo) = PhysicsUtils.Raycast(rayStart,
                            rayEnd,
                            world.CollisionWorld, filter);

                        rayResults[i] = hitInfo;
                    }
                    

                    // Calculate a simple slip factor based on chassis tilt.
                    var slopeSlipFactor = math.pow(math.abs(math.dot(ceUp, math.up())), 4.0f);

                    // Proportional apply velocity changes to each wheel
                    var invWheelCount = 1.0f / wheelsBuffer.Length;
                    for (var i = 0; i < wheelsBuffer.Length; i++)
                    {
                        var weEn = wheelsBuffer[i].Wheel;

                        if (!rotationFilter.HasComponent(weEn) || !translationFilter.HasComponent(weEn) ||
                            !parentFilter.HasComponent(weEn)) continue;

                        var weParent = parentFilter[weEn].Value;
                        var weParentLocalToWorld = localToWorldFilter[weParent];
                        var weRot = rotationFilter[weEn];
                        var weTrans = translationFilter[weEn];
                        var weLocToWorld = localToWorldFilter[weEn];

                        var rayStart = weParentLocalToWorld.Position;
                        var rayEnd = (-ceUp * (mechanics.SuspensionLength + mechanics.WheelBase)) + rayStart;

                        var rayResult = rayResults[i];

                        var wheelPos = rayResult.Position;
                        wheelPos -= (cePosition - ceCenterOfMass);

                        var velocityAtWheel = world.GetLinearVelocity(ceIdx, wheelPos);

                        var weUp = ceUp;
                        var weRight = ceRight;
                        var weForward = ceForward;

                        #region handle wheel steering

                        {
                            if (wheelsBuffer[i].isStearing)
                            {
                                var steeringAngle = math.radians(mechanics.Steer * mechanics.MaxSteerAngle);

                                var wRotation = quaternion.AxisAngle(ceUp, steeringAngle);
                                weRight = math.rotate(wRotation, weRight);
                                weForward = math.rotate(wRotation, weForward);

                                weRot.Value = quaternion.AxisAngle(math.up(), steeringAngle);
                            }
                        }

                        #endregion

                        var currentSpeedUp = math.dot(velocityAtWheel, weUp);
                        var currentSpeedForward = math.dot(velocityAtWheel, weForward);
                        var currentSpeedRight = math.dot(velocityAtWheel, weRight);

                        #region handle wheel rotation
                        if (childFilter.HasComponent(weEn)){
            
                            var rEn = childFilter[weEn][0].Value;

                            if (rotationFilter.HasComponent(rEn))
                            {
                                if (rEn != Entity.Null)
                                {
                                    var isDriven = wheelsBuffer[i].isDriven && Mathf.Abs(mechanics.Acceleration) > 0;

                                    var weRotation = isDriven
                                        ? ((mechanics.MaxAcceleration * mechanics.Acceleration) / mechanics.WheelBase)
                                        : (currentSpeedForward / mechanics.WheelBase);

                                    weRotation = math.radians(weRotation);

                                    var rEnRot = rotationFilter[rEn];
                                    
                                    rEnRot.Value = math.mul(rEnRot.Value, quaternion.AxisAngle(math.right(), weRotation));
                                    
                                    ecb.SetComponent(rEn, rEnRot);
                                }
                            }
                        }

                        #endregion


                        var wheelCurrentPos = weLocToWorld.Position;
                        var hit = !math.all(rayResult.SurfaceNormal == float3.zero);
                        if (!hit)
                        {
                            var wheelDesiredPos = (-ceUp * mechanics.SuspensionLength) + rayStart;
                            weTrans.Value =  math.lerp(wheelCurrentPos, wheelDesiredPos,
                                mechanics.SuspensionDamping / mechanics.SuspensionStrength).ToLocal(weParentLocalToWorld);
                        }
                        else
                        {
                            // remove the wheelbase to get wheel position.
                            var fraction = rayResult.Fraction - (mechanics.WheelBase) /
                                (mechanics.SuspensionLength + mechanics.WheelBase);

                            var wheelDesiredPos = math.lerp(rayStart, rayEnd, fraction);
                            weTrans.Value = math.lerp(wheelCurrentPos, wheelDesiredPos,
                                mechanics.SuspensionDamping / mechanics.SuspensionStrength).ToLocal(weParentLocalToWorld);

                            #region Suspension

                            {
                                // Calculate and apply the impulses
                                var posA = rayEnd;
                                var posB = rayResult.Position;
                                var lvA = currentSpeedUp * weUp; 
                                var lvB = world.GetLinearVelocity(rayResult.RigidBodyIndex, posB);

                                var impulse = mechanics.SuspensionStrength * (posB - posA) +
                                              mechanics.SuspensionDamping * (lvB - lvA);
                                impulse = impulse * invWheelCount;
                                var impulseUp = math.dot(impulse, weUp);

                                // Suspension shouldn't necessarily pull the vehicle down!
                                var downForceLimit = -0.25f;
                                if (downForceLimit < impulseUp)
                                {
                                    impulse = impulseUp * weUp;

                                    world.ApplyImpulse(ceIdx, impulse, posA);

                                    #if UNITY_EDITOR
                                    if (mechanics.DrawDebugInformation)
                                        Debug.DrawRay(wheelDesiredPos, impulse, Color.green);
                                    #endif
                                }
                            }

                            #endregion

                            // #region Sideways friction
                            //
                            // {
                            //     var deltaSpeedRight = (0.0f - currentSpeedRight);
                            //     deltaSpeedRight = math.clamp(deltaSpeedRight, -mechanics.WheelMaxImpulseRight,
                            //         mechanics.WheelMaxImpulseRight);
                            //     deltaSpeedRight *= mechanics.WheelFrictionRight;
                            //     deltaSpeedRight *= slopeSlipFactor;
                            //
                            //     var impulse = deltaSpeedRight * weRight;
                            //     var effectiveMass = world.GetEffectiveMass(ceIdx, impulse, wheelPos);
                            //     impulse = impulse * effectiveMass * invWheelCount;
                            //
                            //     world.ApplyImpulse(ceIdx, impulse, wheelPos);
                            //     world.ApplyImpulse(rayResult.RigidBodyIndex, -impulse, wheelPos);
                            //
                            //     #if UNITY_EDITOR
                            //     if (mechanics.DrawDebugInformation)
                            //         Debug.DrawRay(wheelDesiredPos, impulse, Color.red);
                            //     #endif
                            // }
                            //
                            // #endregion

                            // #region Drive
                            //
                            // {
                            //     if (Mathf.Abs(mechanics.Acceleration) > 0 && wheelsBuffer[i].isDriven)
                            //     {
                            //         var deltaSpeedForward = (mechanics.Acceleration * mechanics.MaxAcceleration - currentSpeedForward);
                            //         deltaSpeedForward = math.clamp(deltaSpeedForward, -mechanics.WheelMaxImpulseForward,
                            //             mechanics.WheelMaxImpulseForward);
                            //         deltaSpeedForward *= mechanics.WheelFrictionForward;
                            //         deltaSpeedForward *= slopeSlipFactor;
                            //
                            //         var impulse = deltaSpeedForward * mechanics.MoveDir;
                            //
                            //         var effectiveMass = world.GetEffectiveMass(ceIdx, impulse, wheelPos);
                            //         impulse = impulse * effectiveMass * invWheelCount;
                            //
                            //         world.ApplyImpulse(ceIdx, impulse, wheelPos);
                            //         world.ApplyImpulse(rayResult.RigidBodyIndex, -impulse, wheelPos);
                            //
                            //         #if UNITY_EDITOR
                            //         if (mechanics.DrawDebugInformation)
                            //             Debug.DrawRay(wheelDesiredPos, impulse, Color.blue);
                            //         #endif
                            //     }
                            // }

                            // #endregion 
                        } 

                        ecb.SetComponent(weEn, weRot);
                        ecb.SetComponent(weEn, weTrans);
                    }

                    rayResults.Dispose();
                }).WithReadOnly(localToWorldFilter).WithReadOnly(translationFilter).WithReadOnly(rotationFilter)
                .WithReadOnly(parentFilter).WithReadOnly(childFilter).Schedule();
        }
    }

    #endregion
}
