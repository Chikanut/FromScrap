using Cars.View.Components;
using Unity.Entities;
using Unity.Transforms;
using Vehicles.Wheels.Components;

[UpdateInGroup( typeof(PresentationSystemGroup) )]
public partial class CarWheelTrailEffectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var groundInfoFilter = GetComponentDataFromEntity<GroundInfoData>(true);
        
        Entities.ForEach((
            Entity entity,
            in ViewData wheelData,
            in GameObjectTrackEntityComponent trailEffectTrackComponent 
        ) =>
        {
            var groundInfoData = groundInfoFilter[wheelData.Parent];
            var isGrounded = groundInfoData.isGrounded;
            var trailPos = groundInfoData.Info.Position;

            trailEffectTrackComponent.InitTrackingGO(isGrounded);
            trailEffectTrackComponent.UpdateTrackingGOPosition(trailPos);
        }).WithReadOnly(groundInfoFilter).WithoutBurst().Run();
    } 
}