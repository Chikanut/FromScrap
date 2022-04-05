using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
        Dependency = Entities.WithAll<BaseAIControllerComponent,CharacterControllerComponent>().ForEach((
            Entity entity,
            ref CharacterControllerComponent controller, in Translation  translation, in HasTarget target) =>
        {
            if (target.TargetEntity == Entity.Null )
            {
                controller.CurrentMagnitude = 0.0f;
                return;
            }
        
            var dir = math.normalize(target.TargetPosition - translation.Value);
            dir.y = 0; 

            controller.CurrentDirection = dir;
            controller.CurrentMagnitude = 1.0f;

            controller.Jump = target.TargetPosition.y > (translation.Value.y + 1.5f);
        }).ScheduleParallel(_findClosestTargetSystem.FindTargetHandle);
    }
}
