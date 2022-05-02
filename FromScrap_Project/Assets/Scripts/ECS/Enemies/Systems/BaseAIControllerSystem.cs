using Reese.Math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FindClosestTargetSystem))]
public partial class BaseAIControllerSystem : SystemBase
{
    private FindClosestTargetSystem _findClosestTargetSystem;

    protected override void OnCreate()
    {
        _findClosestTargetSystem = World.GetOrCreateSystem<FindClosestTargetSystem>();
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        Dependency = Entities.WithAll<BaseGroundAIControllerComponent>().ForEach((
            ref CharacterControllerInput controller, in LocalToWorld localToWorld, in HasTarget target) =>
        {
            if (target.TargetEntity == Entity.Null )
            {
                // Debug.Log("No target");
                return;
            }
        
            var dir = math.normalize(target.TargetPosition - localToWorld.Position);
            dir.y = 0;
            var dirPower = math.clamp(math.length(dir), 0, 1);
            var forward = localToWorld.Forward;
            var dirDot = math.clamp(math.dot(forward, dir)-0.5f, 0, 1);
            var angle = math.clamp(dir.AngleSigned(forward, math.up())/180, -1, 1);

            var resultDir = forward * dirDot;

            controller.Movement = resultDir;
            controller.Rotation = -angle;
        }).ScheduleParallel(_findClosestTargetSystem.FindTargetHandle);
    }
}
