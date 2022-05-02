using Reese.Math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

using VertexFragment;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class CheckGroundSystem : SystemBase
{
    private BuildPhysicsWorld _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    
    }
    
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        var collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
        
        Entities.ForEach((ref GroundInfoData groundInfoData, in LocalToWorld localToWorld) =>
        {
            var downDir = groundInfoData.isLocalDown ? localToWorld.Up : math.up();
            var startPos = groundInfoData.CheckOffset.ToWorld(localToWorld);
            
            var (isHit, hitInfo) = PhysicsUtils.Raycast(startPos,
                startPos - downDir * groundInfoData.CheckDistance,
                collisionWorld, groundInfoData.CollisionFilter);
            
            groundInfoData.isGrounded = isHit;
            groundInfoData.Info = hitInfo;
  
        }).WithReadOnly(collisionWorld).ScheduleParallel();
    }
}
