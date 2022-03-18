using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RotateTowardsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithAll<RotateTowards>().ForEach((ref Rotation rotation, in RotateTowards rotateTowards, in Translation translation) =>
        {
            var dir = math.normalize(rotateTowards.Target - translation.Value);
            
            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotation(dir, new float3(0, 1, 0)), rotateTowards.RotationSpeed * deltaTime);
        }).Schedule();
    }
}