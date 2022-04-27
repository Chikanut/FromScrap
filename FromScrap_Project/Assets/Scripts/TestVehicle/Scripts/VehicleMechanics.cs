using Reese.Math;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using VertexFragment;

namespace Demos
{
    public struct VehicleMechanics : IComponentData
    {
        public float3 chassisUp;
        public float3 chassisRight;
        public float3 chassisForward;
        public float wheelBase;
        public float wheelFrictionRight;
        public float wheelFrictionForward;
        public float wheelMaxImpulseRight;
        public float wheelMaxImpulseForward;
        public float suspensionLength;
        public float suspensionStrength;
        public float suspensionDamping;
        public float steeringAngle;
        public bool driveEngaged;
        public float driveDesiredSpeed;
        public bool drawDebugInformation;
        public CollisionFilter collisionFilter;
    }

    public struct WheelsBuffer : IBufferElementData
    {
        public Entity Wheel;
        public bool isStearing;
        public bool isDriven;
    }

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

            Entities.ForEach((Entity entity, in Translation translation, in Rotation rotation, in VehicleMechanics mechanics, in DynamicBuffer<WheelsBuffer> wheelsBuffer, in LocalToWorld localToWorld) =>
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
                    var ceRotation = rotation.Value;
                    var ceCenterOfMass = world.GetCenterOfMass(ceIdx);

                    var ceUp = localToWorld.Up;//math.mul(ceRotation, mechanics.chassisUp);
                    var ceForward = localToWorld.Forward;// math.mul(ceRotation, mechanics.chassisForward);
                    var ceRight = localToWorld.Right;// math.mul(ceRotation, mechanics.chassisRight);

                    var rayResults = new NativeArray<RaycastHit>(wheelsBuffer.Length, Allocator.Temp);

                    var filter = mechanics.collisionFilter;
                    for (var i = 0; i < wheelsBuffer.Length; i++)
                    {
                        var weEn = wheelsBuffer[i].Wheel;
                    
                        if (!parentFilter.HasComponent(weEn)) continue;
                    
                        var rayStart = localToWorldFilter[parentFilter[weEn].Value].Position;
                        var rayEnd = (-ceUp * (mechanics.suspensionLength + mechanics.wheelBase)) + rayStart;
                    
                        if (mechanics.drawDebugInformation)
                            Debug.DrawRay(rayStart, rayEnd - rayStart);
                    
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
                        var rayEnd = (-ceUp * (mechanics.suspensionLength + mechanics.wheelBase)) + rayStart;

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
                                var steeringAngle = math.radians(mechanics.steeringAngle);

                                var wRotation = quaternion.AxisAngle(ceUp, steeringAngle);
                                weRight = math.rotate(wRotation, weRight);
                                weForward = math.rotate(wRotation, weForward);

                                weRot.Value = quaternion.AxisAngle(mechanics.chassisUp, steeringAngle);
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
                                    var isDriven = (mechanics.driveEngaged && wheelsBuffer[i].isDriven);

                                    var weRotation = isDriven
                                        ? (mechanics.driveDesiredSpeed / mechanics.wheelBase)
                                        : (currentSpeedForward / mechanics.wheelBase);

                                    weRotation = math.radians(weRotation);

                                    var rEnRot = rotationFilter[rEn];
                                    
                                    rEnRot.Value = math.mul(rEnRot.Value, quaternion.AxisAngle(mechanics.chassisRight, weRotation));
                                    
                                    ecb.SetComponent(rEn, rEnRot);
                                }
                            }
                        }

                        #endregion


                        var wheelCurrentPos = weLocToWorld.Position;
                        var hit = !math.all(rayResult.SurfaceNormal == float3.zero);
                        if (!hit)
                        {
                            var wheelDesiredPos = (-ceUp * mechanics.suspensionLength) + rayStart;
                            weTrans.Value =  math.lerp(wheelCurrentPos, wheelDesiredPos,
                                mechanics.suspensionDamping / mechanics.suspensionStrength).ToLocal(weParentLocalToWorld);
                        }
                        else
                        {
                            // remove the wheelbase to get wheel position.
                            var fraction = rayResult.Fraction - (mechanics.wheelBase) /
                                (mechanics.suspensionLength + mechanics.wheelBase);

                            var wheelDesiredPos = math.lerp(rayStart, rayEnd, fraction);
                            weTrans.Value = math.lerp(wheelCurrentPos, wheelDesiredPos,
                                mechanics.suspensionDamping / mechanics.suspensionStrength).ToLocal(weParentLocalToWorld);

                            #region Suspension

                            {
                                // Calculate and apply the impulses
                                var posA = rayEnd;
                                var posB = rayResult.Position;
                                var lvA = currentSpeedUp * weUp; 
                                var lvB = world.GetLinearVelocity(rayResult.RigidBodyIndex, posB);

                                var impulse = mechanics.suspensionStrength * (posB - posA) +
                                              mechanics.suspensionDamping * (lvB - lvA);
                                impulse = impulse * invWheelCount;
                                var impulseUp = math.dot(impulse, weUp);

                                // Suspension shouldn't necessarily pull the vehicle down!
                                var downForceLimit = -0.25f;
                                if (downForceLimit < impulseUp)
                                {
                                    impulse = impulseUp * weUp;

                                    world.ApplyImpulse(ceIdx, impulse, posA);

                                    if (mechanics.drawDebugInformation)
                                        Debug.DrawRay(wheelDesiredPos, impulse, Color.green);
                                }
                            }

                            #endregion

                            #region Sideways friction

                            {
                                var deltaSpeedRight = (0.0f - currentSpeedRight);
                                deltaSpeedRight = math.clamp(deltaSpeedRight, -mechanics.wheelMaxImpulseRight,
                                    mechanics.wheelMaxImpulseRight);
                                deltaSpeedRight *= mechanics.wheelFrictionRight;
                                deltaSpeedRight *= slopeSlipFactor;

                                var impulse = deltaSpeedRight * weRight;
                                var effectiveMass = world.GetEffectiveMass(ceIdx, impulse, wheelPos);
                                impulse = impulse * effectiveMass * invWheelCount;

                                world.ApplyImpulse(ceIdx, impulse, wheelPos);
                                world.ApplyImpulse(rayResult.RigidBodyIndex, -impulse, wheelPos);

                                if (mechanics.drawDebugInformation)
                                    Debug.DrawRay(wheelDesiredPos, impulse, Color.red);
                            }

                            #endregion

                            #region Drive

                            {
                                if (mechanics.driveEngaged && wheelsBuffer[i].isDriven)
                                {
                                    var deltaSpeedForward = (mechanics.driveDesiredSpeed - currentSpeedForward);
                                    deltaSpeedForward = math.clamp(deltaSpeedForward, -mechanics.wheelMaxImpulseForward,
                                        mechanics.wheelMaxImpulseForward);
                                    deltaSpeedForward *= mechanics.wheelFrictionForward;
                                    deltaSpeedForward *= slopeSlipFactor;

                                    var impulse = deltaSpeedForward * weForward;

                                    var effectiveMass = world.GetEffectiveMass(ceIdx, impulse, wheelPos);
                                    impulse = impulse * effectiveMass * invWheelCount;

                                    world.ApplyImpulse(ceIdx, impulse, wheelPos);
                                    world.ApplyImpulse(rayResult.RigidBodyIndex, -impulse, wheelPos);

                                    if (mechanics.drawDebugInformation)
                                        Debug.DrawRay(wheelDesiredPos, impulse, Color.blue);
                                }
                            }

                            #endregion 
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
