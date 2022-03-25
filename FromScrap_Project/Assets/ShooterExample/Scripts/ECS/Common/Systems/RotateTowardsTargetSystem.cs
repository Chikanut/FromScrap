using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

public partial class RotateTowardsTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        
        Entities.ForEach((ref Rotation rotation, ref RotateTowardsTarget rotateTowards, in LocalToWorld localToWorld, in HasTarget target) =>
        {
            if (target.TargetEntity == Entity.Null)
            {
                rotateTowards.IsRotated = false;
                return;
            }

            var targetTransform = GetComponent<LocalToWorld>(target.TargetEntity);
            var targetPosition = targetTransform.Position;
            
            if(!rotateTowards.VerticalRotation)
                targetPosition.y = localToWorld.Position.y;
            
            var dir = targetPosition - localToWorld.Position;
            
            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(dir, new float3(0, 1, 0)), rotateTowards.RotationSpeed * time);
            
            var angle = dir.Angle(localToWorld.Forward);
            
            rotateTowards.IsRotated = angle < rotateTowards.IsRotatedRadius;
        }).ScheduleParallel();
    }
}
