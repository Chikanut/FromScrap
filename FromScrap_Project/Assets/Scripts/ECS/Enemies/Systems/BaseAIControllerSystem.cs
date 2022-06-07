using ECS.FindTargetSystem;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BaseAIControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref CharacterControllerInput controller,
            in BaseGroundAIControllerComponent aiControllerComponent, in LocalToWorld localToWorld,
            in HasTarget target) =>
        {
            var forward = localToWorld.Forward;
            forward.y = 0;
            
            if (target.TargetEntity == Entity.Null)
            {
                controller.Movement = forward;
                controller.Rotation = 0;
                return;
            }

            var dir = math.normalize(target.TargetPosition - localToWorld.Position);
            dir.y = 0;
          

            var angle = dir.AngleSigned(forward, math.up());
            var rotationDir = math.clamp(angle / 180, -1, 1);

            var resultDir = forward * (math.abs(angle) < aiControllerComponent.MaxMovementDir ? 1 : 0);

            controller.Movement = resultDir;
            controller.Rotation = -rotationDir;
        }).ScheduleParallel();
    }
}
