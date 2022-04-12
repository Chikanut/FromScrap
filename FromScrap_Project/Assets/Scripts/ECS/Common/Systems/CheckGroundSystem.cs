using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
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
            var downDir = groundInfoData.isLocalDown ? localToWorld.Up : math.up();
            var (isHit, hitInfo) = PhysicsUtils.Raycast(localToWorld.Position,
                localToWorld.Position - downDir * groundInfoData.CheckDistance,
                collisionWorld, groundInfoData.CollisionFilter);
            groundInfoData.isGrounded = isHit;
            groundInfoData.GroundPosition = hitInfo.Position;
            groundInfoData.GroundNormal = hitInfo.SurfaceNormal;
        }).WithReadOnly(collisionWorld).ScheduleParallel();
        
        Entities.ForEach((ref DynamicBuffer<MultyGroundInfoData> groundInfoData, in LocalToWorld localToWorld) =>
        {
            for (var i = 0; i < groundInfoData.Length; i++)
            {
                var downDir = groundInfoData[i].isLocalDown ? localToWorld.Up : math.up();
                var startPoint = localToWorld.Value.LocalToWorld(groundInfoData[i].AnchorPoints);
                var (isHit, hitInfo) = PhysicsUtils.Raycast(startPoint,
                    startPoint - downDir * groundInfoData[i].CheckDistance,
                    collisionWorld,
                    groundInfoData[i].CollisionFilter);

                // if (isHit)
                // {
                //     Debug.DrawLine(localToWorld.Value.LocalToWorld(groundInfoData[i].AnchorPoints), hitInfo.Position,
                //         Color.green);
                // }

                groundInfoData[i] = new MultyGroundInfoData()
                {
                    AnchorPoints = groundInfoData[i].AnchorPoints,
                    CheckDistance = groundInfoData[i].CheckDistance,
                    CollisionFilter = groundInfoData[i].CollisionFilter,
                    isGrounded = isHit,
                    GroundPosition = hitInfo.Position,
                    GroundNormal = hitInfo.SurfaceNormal,
                    isLocalDown = groundInfoData[i].isLocalDown
                };
            }
        }).WithReadOnly(collisionWorld).ScheduleParallel();
    }
}
