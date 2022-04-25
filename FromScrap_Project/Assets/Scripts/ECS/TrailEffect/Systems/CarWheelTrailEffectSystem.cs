using Cars.View.Components;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup( typeof(PresentationSystemGroup) )]
public partial class CarWheelTrailEffectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            Entity entity,
            in GroundInfoData groundInfoData,
            in WheelData wheelData,
            in GameObjectTrackEntityComponent trailEffectTrackComponent 
        ) =>
        {
            var isGrounded = groundInfoData.isGrounded;
            var trailPos = groundInfoData.GroundPosition;

            trailEffectTrackComponent.InitTrackingGO(isGrounded);
            trailEffectTrackComponent.UpdateTrackingGOPosition(trailPos);
        }).WithoutBurst().Run();
    } 
}