using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RotateTowardsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities.WithAll<HasTarget>().ForEach((ref Rotation rotation, in HasTarget targetInfo, in Translation translation) =>
        {
            if(targetInfo.TargetEntity == Entity.Null) return;
            
            var dir = math.normalize(targetInfo.TargetPosition - translation.Value);

            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotation(dir, new float3(0, 1, 0)),
                5 * deltaTime);
            
        }).Schedule();
    }
}