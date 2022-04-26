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
        Dependency = Entities.WithAll<BaseAIControllerComponent>().ForEach((
            ref CharacterControllerInternalData controller, in LocalToWorld localToWorld, in HasTarget target) =>
        {
            if (target.TargetEntity == Entity.Null )
            {
                // Debug.Log("No target");
                return;
            }
        
            var dir = math.normalize(target.TargetPosition - localToWorld.Position);
            dir.y = 0;
            var forward = localToWorld.Forward;
            var angle = math.clamp(dir.AngleSigned(forward, math.up())/180, -1, 1);
            
            controller.Input.Movement = new float2(dir.x, dir.z);
            controller.Input.Rotation = -angle;
        }).ScheduleParallel(_findClosestTargetSystem.FindTargetHandle);
    }
}
