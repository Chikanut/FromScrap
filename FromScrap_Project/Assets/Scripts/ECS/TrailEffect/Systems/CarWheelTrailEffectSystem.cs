using Cars.View.Components;
using Unity.Entities;
using Unity.Transforms;
using Vehicles.Wheels.Components;

[UpdateInGroup( typeof(PresentationSystemGroup) )]
public partial class CarWheelTrailEffectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            Entity entity,
            in GroundInfoData groundInfoData,
            in ViewData wheelData,
            in GameObjectTrackEntityComponent trailEffectTrackComponent 
        ) =>
        {
            var isGrounded = groundInfoData.isGrounded;
            var trailPos = groundInfoData.Info.Position;

            trailEffectTrackComponent.InitTrackingGO(isGrounded);
            trailEffectTrackComponent.UpdateTrackingGOPosition(trailPos);
        }).WithoutBurst().Run();
    } 
}