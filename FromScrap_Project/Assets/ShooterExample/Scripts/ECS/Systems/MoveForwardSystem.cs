using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MoveForwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithAll<MoveForward>().ForEach((ref Translation translation, in Rotation rotation, in MoveForward moveForward) => {
            translation.Value += moveForward.Speed * deltaTime * math.forward(rotation.Value);
        }).Schedule();
    }
}
