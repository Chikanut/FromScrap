using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

public partial class GameCharacterMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<
            GameCharacterMovementComponent>().ForEach((
            ref GameCharacterMovementComponent movementComponent,
            ref PhysicsVelocity velocity,
            ref PhysicsMass mass,
            ref LocalToWorld localToWorld,
            ref Rotation rotation
        ) => {
            
            movementComponent.CurrentSpeed = math.sqrt(math.pow(movementComponent.CurrentVelocity.x, 2) + math.pow(movementComponent.CurrentVelocity.y, 2) + math.pow(movementComponent.CurrentVelocity.z, 2));
            movementComponent.SpeedParameter = movementComponent.MaxMotorTorque * movementComponent.MaxAcceleration * movementComponent.VerticalAxis * (1 - (movementComponent.CurrentSpeed / movementComponent.MaxSpeed));
            movementComponent.SteerParameter = movementComponent.MaxSteerAngle * movementComponent.HorizontalAxis;
           
            if (movementComponent.SpaceKey)
            {
                movementComponent.BreakParameter = movementComponent.MaxBreakTorque * (movementComponent.CurrentSpeed / movementComponent.MaxSpeed) * movementComponent.MaxAcceleration;
                velocity.Linear.y = 2f;
            }
            else
            {
                movementComponent.BreakParameter = 0;
            }
            
            //float2 newVel = vel.Linear.xz;
            //float2 curInput = new float2(movementComponent.HorizontalAxis, movementComponent.VerticalAxis);
            //newVel += curInput * movementComponent.MaxSpeed * deltaTime;
            //vel.Linear.xz = newVel;

            var maxSpeed = movementComponent.MaxSpeed;
            var targetSpeed = maxSpeed * movementComponent.VerticalAxis;
            
            movementComponent.CurrentSpeed = math.lerp(movementComponent.CurrentSpeed, targetSpeed, 0.5f);

            //var currentVelocity = localToWorld.Forward * movementComponent.CurrentSpeed;
            movementComponent.CurrentVelocity = Vector3.Lerp(movementComponent.CurrentVelocity,
                new float3(movementComponent.HorizontalAxis, 0f, movementComponent.VerticalAxis) * movementComponent.MaxSpeed, 0.1f);
            //currentVelocity = math.lerp(currentVelocity, new float3(300f, 0f, 0f) * deltaTime, 0.5f);
            velocity.Linear = new float3(movementComponent.CurrentVelocity.x, velocity.Linear.y, movementComponent.CurrentVelocity.z);


            var dir = math.normalize(velocity.Linear);
            dir.y = 0f;
            
            if(movementComponent.VerticalAxis != 0f || movementComponent.HorizontalAxis != 0f)
                movementComponent.CurrentDirection = dir;
            
            rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(movementComponent.CurrentDirection, 
                new float3(0, 1, 0)), 0.2f);

            
            /*
            var rotationSpeed = 0f;
            
            if (math.abs(movementComponent.CurrentSpeed) > 0.2f)
            {
                rotationSpeed = movementComponent.HorizontalAxis * movementComponent.RotationSpeed;
            }

            var angular = new float3(0f, rotationSpeed, 0f);
            //Change the steering direction on reverse
            if (movementComponent.VerticalAxis < 0)
            {
                angular = -angular;
            }

            //var q = rotation.Value;
            //var angle = 2.0f * math.atan(q.value.y / q.value.w);
            //rotation.Value = quaternion.RotateY(angle);

            //velocity.SetAngularVelocityWorldSpace(mass, rotation, angular);
            */
        }).ScheduleParallel();
    }
}
