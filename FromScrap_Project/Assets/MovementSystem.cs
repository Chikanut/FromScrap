using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref MovementComponent movementComponent, ref Translation translation) => Execute(ref movementComponent, ref translation));
    }

    void Execute(ref MovementComponent movementInfo, ref Translation translation)
    {
        translation.Value.z += movementInfo.MovementSpeed * Time.DeltaTime;

        if (translation.Value.z > movementInfo.Limits.y)
        {
            movementInfo.MovementSpeed = -math.abs(movementInfo.MovementSpeed);
        }else if (translation.Value.z < movementInfo.Limits.x)
        {
            movementInfo.MovementSpeed = math.abs(movementInfo.MovementSpeed);
        }
    }
}
