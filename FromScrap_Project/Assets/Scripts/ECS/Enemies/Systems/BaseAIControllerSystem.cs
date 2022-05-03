using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BaseAIControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BaseGroundAIControllerComponent>().ForEach((
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
        }).ScheduleParallel();
    }
}
