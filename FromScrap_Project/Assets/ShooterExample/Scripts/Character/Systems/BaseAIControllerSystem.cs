using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using VertexFragment;

public partial class BaseAIControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BaseAIControllerComponent,CharacterControllerComponent>().ForEach((
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
        }).ScheduleParallel();
    }
}
