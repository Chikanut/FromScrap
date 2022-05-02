using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateAfter(typeof(BuildPhysicsWorld)), UpdateBefore(typeof(StepPhysicsWorld))]
public partial class CharacterControllerSystem : SystemBase
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

        var world = _createPhysicsWorldSystem.PhysicsWorld;
        
        Entities.ForEach((Entity entity, ref Rotation rotation, in CharacterControllerInput input, in CharacterControllerComponentData data, in GroundInfoData groundInfoData, in LocalToWorld body) =>
        {
            var ceIdx = world.GetRigidBodyIndex(entity);

            if (-1 == ceIdx || ceIdx >= world.NumDynamicBodies)
            {
                return;
            }
            
            if (!groundInfoData.isGrounded) return;

            var cePosition = body.Position;
            var ceCenterOfMass = world.GetCenterOfMass(ceIdx);
            var ceUp = body.Up;
            
            var contactPos = groundInfoData.Info.Position;
            contactPos -= (cePosition - ceCenterOfMass);
            
            var velocityAtContact = world.GetLinearVelocity(ceIdx, contactPos);
            var slopeSlipFactor = math.pow(math.abs(math.dot(ceUp, math.up())), 4.0f);

            var dir = input.Movement;
            var movementPower = math.clamp(math.length(dir),0,1);
            
            var weRight = math.cross(groundInfoData.Info.SurfaceNormal, body.Forward);
            var groundedDir = math.cross(weRight, groundInfoData.Info.SurfaceNormal);

        /*    #region Leveling
            {
                var level = math.dot(groundInfoData.Info.SurfaceNormal, ceUp);
                var levelPower =  1 - math.clamp(level, 0, 1);
                
                var levelPowerPosition = ceCenterOfMass + (ceUp * 2.0f);
                var levelDir = math.lerp(groundInfoData.Info.SurfaceNormal, groundInfoData.Info.SurfaceNormal - ceUp,
                    levelPower);
                
                Debug.DrawRay(groundInfoData.Info.Position, groundInfoData.Info.SurfaceNormal, Color.red);
                Debug.DrawRay(levelPowerPosition, groundInfoData.Info.SurfaceNormal, Color.green);
                Debug.DrawRay(levelPowerPosition + groundInfoData.Info.SurfaceNormal, levelDir * levelPower, Color.blue);
                
                var impulse = data.LevelingPower * levelDir * levelPower * world.GetMass(ceIdx);
                
                world.ApplyImpulse(ceIdx, impulse, levelPowerPosition);
            }
            
            #endregion*/
            
            #region Sideways
            {
                var deltaSpeedRight = (0.0f - (math.dot(velocityAtContact, weRight)));
                deltaSpeedRight = math.clamp(deltaSpeedRight, -data.MaxSidewaysImpulse, data.MaxSidewaysImpulse);
                deltaSpeedRight *= slopeSlipFactor;
                var impulse = deltaSpeedRight * weRight;
                var effectiveMass = world.GetEffectiveMass(ceIdx, impulse, contactPos);
                impulse *= effectiveMass;
                    
                world.ApplyImpulse(ceIdx, impulse, groundInfoData.Info.Position);
            }
                
            #endregion
            
            #region Forward        
            {
                var currentSpeedForward = math.dot(velocityAtContact, groundedDir);
                var deltaSpeedForward =
                    math.clamp(data.MaxSpeed * movementPower - currentSpeedForward, 0, data.MaxSpeed);
                deltaSpeedForward *= data.Acceleration;
                deltaSpeedForward = math.clamp(deltaSpeedForward, -data.MaxAcceleration, data.MaxAcceleration);
                deltaSpeedForward *= slopeSlipFactor;

                var driveImpulse = deltaSpeedForward * groundedDir;
                
                world.ApplyImpulse(ceIdx, driveImpulse, groundInfoData.Info.Position);
            }
            #endregion

            #region Rotate
            {
                world.SetAngularVelocity(ceIdx, ceUp * input.Rotation * data.RotationSpeed);
            }
            #endregion
            
        }).Schedule();

    }
}