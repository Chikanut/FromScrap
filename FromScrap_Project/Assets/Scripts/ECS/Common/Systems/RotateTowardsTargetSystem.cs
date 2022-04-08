using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

            var targetTransform = GetComponent<LocalToWorld>(target.TargetEntity);
            var targetPosition = targetTransform.Position;
            
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
                    
                    // angle = dir.Angle(localToWorld.Forward);
                }
            }
 
            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(dir, up), rotateTowards.RotationSpeed * time);
            
            var angle = dir.Angle(localToWorld.Forward);
            // Debug.Log(angle);
            
            rotateTowards.IsRotated = angle < rotateTowards.IsRotatedRadius;
        }).WithReadOnly(ltw).WithReadOnly(prnt).ScheduleParallel();
    }
}
