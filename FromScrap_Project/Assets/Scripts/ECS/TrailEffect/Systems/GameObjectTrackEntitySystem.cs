using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup( typeof(PresentationSystemGroup) )]
public partial class GameObjectTrackEntitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            Entity entity,
            ref LocalToWorld localToWorld,
            in GameObjectTrackComponent trailEffectTrackComponent 
        ) =>
        {
            var posE = localToWorld.Position;
            
            trailEffectTrackComponent.UpdateEntityPos(posE);
            trailEffectTrackComponent.SpawnGo();

        }).WithoutBurst().Run();
    } 
}