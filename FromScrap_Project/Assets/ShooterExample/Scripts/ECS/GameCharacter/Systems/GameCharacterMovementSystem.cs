using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public partial class GameCharacterMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<
            GameCharacterMovementComponent>().ForEach((
            ref GameCharacterMovementComponent movementComponent,
            ref PhysicsVelocity vel) => {
            
            movementComponent.CurrentSpeed = math.sqrt(math.pow(movementComponent.CurrentVelocity.x, 2) + math.pow(movementComponent.CurrentVelocity.y, 2) + math.pow(movementComponent.CurrentVelocity.z, 2));
            movementComponent.SpeedParameter = movementComponent.MaxMotorTorque * movementComponent.MaxAcceleration * movementComponent.VerticalAxis * (1 - (movementComponent.CurrentSpeed / movementComponent.MaxSpeed));
            movementComponent.SteerParameter = movementComponent.MaxSteerAngle * movementComponent.HorizontalAxis;
           
            if (movementComponent.SpaceKey)
            {
                movementComponent.BreakParameter = movementComponent.MaxBreakTorque * (movementComponent.CurrentSpeed / movementComponent.MaxSpeed) * movementComponent.MaxAcceleration;
                vel.Linear.y = 2f;
            }
            else
            {
                movementComponent.BreakParameter = 0;
            }
            
            float2 newVel = vel.Linear.xz;
            float2 curInput = new float2(movementComponent.HorizontalAxis, movementComponent.VerticalAxis);
            
            newVel += curInput * movementComponent.MaxSpeed * deltaTime;
            vel.Linear.xz = newVel;
            
        }).ScheduleParallel();
    }
}
