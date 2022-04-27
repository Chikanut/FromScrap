using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Vehicles.Components;

namespace Vehicles.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup)), UpdateBefore(typeof(WheelsSystem))]
    public partial class VehicleMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref VehicleInputComponent inputComponent, ref PhysicsVelocity velocity, in LocalToWorld localToWorld) =>
            {
                var newDirection = new float3(inputComponent.MoveDir.x, 0f, inputComponent.MoveDir.z);
                newDirection = Vector3.ClampMagnitude(newDirection, 1f);

                inputComponent.CurrentVelocity = newDirection * 10;
                
                velocity.Linear = new float3(
                    inputComponent.CurrentVelocity.x,
                    velocity.Linear.y,
                    inputComponent.CurrentVelocity.z);
                
                var dir = new float3(0f, 0f, 0f);

                if (Vector3.Magnitude(newDirection) > 0)
                    dir = Vector3.Normalize(velocity.Linear);

                dir.y = 0f;

                var orientTarget = localToWorld.Right;
                var orient = Vector3.Dot(dir, orientTarget);

                orient = Mathf.Clamp(orient, -1f, 1f);

                velocity.Angular.y = orient * 5;
            }).Schedule();
        }
    }
}