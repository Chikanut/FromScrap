using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS_Test
{
    public class MovementSystem : ComponentSystem
    {
        private float4 LevelSize = new float4(22, 5.5f, 20, -20);

        protected override void OnUpdate()
        {
            Entities.ForEach((ref MovementComponent movementComponent, ref Translation translation) =>
                Execute(ref movementComponent, ref translation));
        }

        void Execute(ref MovementComponent movementInfo, ref Translation translation)
        {
            translation.Value += movementInfo.Velocity * Time.DeltaTime;

            // if (translation.Value.z > LevelSize.x)
            // {
            //     movementInfo.Velocity.z = -math.abs(movementInfo.Velocity.z);
            // }
            // else if (translation.Value.z < LevelSize.y)
            // {
            //     movementInfo.Velocity.z = math.abs(movementInfo.Velocity.z);
            // }
            //
            // if (translation.Value.x > LevelSize.z)
            // {
            //     movementInfo.Velocity.x = -math.abs(movementInfo.Velocity.x);
            // }
            // else if (translation.Value.x < LevelSize.w)
            // {
            //     movementInfo.Velocity.x = math.abs(movementInfo.Velocity.x);
            // }
        }
    }
}
