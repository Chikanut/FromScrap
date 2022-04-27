using Demos;
using Unity.Entities;

partial class VehicleSteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("VehicleSteeringJob")
            .ForEach((ref VehicleMechanics mechanics) =>
            {
                float x = 0;
                float a = 1;
                float z = 0;

                mechanics.steeringAngle = 0;
                mechanics.driveDesiredSpeed = 45;
                mechanics.driveEngaged = true;
            }).Schedule();
    }
}
