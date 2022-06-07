using ECS.FindTargetSystem;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RotateTowardsTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        
        var ltw = GetComponentDataFromEntity<LocalToWorld>(true);
        var prnt = GetComponentDataFromEntity<Parent>(true);
        
        Entities.ForEach((Entity entity, ref Rotation rotation, ref RotateTowardsTarget rotateTowards, in LocalToWorld localToWorld, in HasTarget target) =>
        {
            if (target.TargetEntity == Entity.Null)
            {
                rotateTowards.IsRotated = false;
                return;
            }

            if(!ltw.HasComponent(target.TargetEntity)) return;
            
            var targetPosition = ltw[target.TargetEntity].Position;
            
            if(!rotateTowards.VerticalRotation)
                targetPosition.y = localToWorld.Position.y;
            
            var dir = targetPosition - localToWorld.Position;
            var up = new float3(0, 1, 0);
            
            if (prnt.HasComponent(entity))
            {
                var parent =  prnt[entity];

                if (ltw.HasComponent(parent.Value))
                {
                    var parentTransform = ltw[parent.Value];
                    
                    dir = parentTransform.Value.WorldToLocal(dir + parentTransform.Position);
                    up = parentTransform.Up;
                }
            }
 
            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(dir, up), rotateTowards.RotationSpeed * time);

            var angleDir = targetPosition - localToWorld.Position;
            var angle = angleDir.Angle(localToWorld.Forward);

            rotateTowards.IsRotated = angle < rotateTowards.IsRotatedRadius;
        }).WithReadOnly(ltw).WithReadOnly(prnt).ScheduleParallel();
    }
}
