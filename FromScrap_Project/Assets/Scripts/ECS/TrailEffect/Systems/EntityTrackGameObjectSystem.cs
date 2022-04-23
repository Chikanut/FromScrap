using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class EntityTrackGameObjectSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((
            Entity entity,
            ref LocalToWorld localToWorld,
            in GameObjectTrackComponent trailEffectTrackComponent 
            ) =>
        {
            //var pos = Vector3.Lerp(localToWorld.Position,trailEffectTrackComponent.TargetObject.position, 0.05f);
            //var rot = trailEffectTrackComponent.TargetObject.rotation;
           
            //localToWorld.Value = new float4x4(rot, pos);
        }).WithoutBurst().Run();
    } 
}
