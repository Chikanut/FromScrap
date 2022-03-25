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

    protected override void OnUpdate()
    {
        var collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;

        Entities.ForEach((ref GroundInfoData groundInfoData, in LocalToWorld localToWorld) =>
        {
            var (isHit, hitInfo) = PhysicsUtils.Raycast(localToWorld.Position,
                localToWorld.Position - new float3(0, groundInfoData.CheckDistance, 0), groundInfoData.CollisionFilter,
                collisionWorld);
            groundInfoData.isGrounded = isHit;
            groundInfoData.GroundPosition = hitInfo.Position;
            groundInfoData.GroundNormal = hitInfo.SurfaceNormal;
        }).WithReadOnly(collisionWorld).ScheduleParallel();
    }
}
