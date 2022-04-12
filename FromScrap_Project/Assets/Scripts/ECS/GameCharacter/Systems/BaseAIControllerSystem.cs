using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VertexFragment;

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
            ref GameCharacterMovementComponent controller, in Translation  translation, in HasTarget target) =>
        {
            if (target.TargetEntity == Entity.Null )
            {
                // Debug.Log("No target");
                return;
            }
        
            var dir = math.normalize(target.TargetPosition - translation.Value);
            dir.y = 0; 
            
            controller.HorizontalAxis = dir.x;
            controller.VerticalAxis = dir.z;
        }).ScheduleParallel(_findClosestTargetSystem.FindTargetHandle);
    }
}
