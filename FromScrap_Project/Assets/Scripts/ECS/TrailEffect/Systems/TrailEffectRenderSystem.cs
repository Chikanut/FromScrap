using System.Collections;
using System.Collections.Generic;
using Cars.View.Components;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TrailEffectCalculateSystem))]
[UpdateInGroup( typeof(PresentationSystemGroup) )]
    
public partial class TrailEffectRenderSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;
    
    protected override void OnCreate() {
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        base.OnCreate();
    }
    
    protected override void OnUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        float deltaTime = Time.DeltaTime;

        var groundInfo = GetComponentDataFromEntity<GroundInfoData>(true);
        var multyGroundInfo = GetBufferFromEntity<MultyGroundInfoData>(true);
        
        ComponentDataFromEntity<Translation> myTypeFromEntity = GetComponentDataFromEntity<Translation>(true);
        
        var ecb = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.
            WithAll<TrailEffectData>().
            ForEach((
                Entity entity,
                ref TrailEffectData trailEffectData,
                in RenderMesh renderMesh
            ) =>
        {
            if(myTypeFromEntity.HasComponent(trailEffectData.TargetEntity))
            {
                var myType = myTypeFromEntity[trailEffectData.TargetEntity];

                //Debug.LogError(myType.Value);
            }
        }).
            WithoutBurst().Run();
            //WithReadOnly(multyGroundInfo).
            //ScheduleParallel();
    }
}
