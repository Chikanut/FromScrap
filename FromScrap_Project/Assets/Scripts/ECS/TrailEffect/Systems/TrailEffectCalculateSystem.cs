using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cars.View.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

//[UpdateBefore(typeof(TrailEffectRenderSystem))]
[UpdateInGroup( typeof(SimulationSystemGroup) )]
    
public partial class TrailEffectCalculateSystem : SystemBase
{
    //private EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;
   
    protected override void OnCreate() {
        //entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        base.OnCreate();
    }
   
    protected override void OnUpdate()
    {
        /*
        float fixedDeltaTime = Time.fixedDeltaTime;
        float deltaTime = Time.DeltaTime;
        var groundInfo = GetComponentDataFromEntity<GroundInfoData>(true);
        var trailEffectBuffer = GetBufferFromEntity<TrailEffectInfoData>(true);
        ComponentDataFromEntity<Translation> myTypeFromEntity = GetComponentDataFromEntity<Translation>(true);
        var ecb = entityCommandBufferSystem.CreateCommandBuffer();
        */
        
        var groundInfo = GetComponentDataFromEntity<GroundInfoData>(true);
        
        Entities.ForEach((
                Entity entity,
                ref DynamicBuffer<TrailEffectInfoData> trailEffectBuffer,
                ref TrailEffectViewComponent trailEffectViewComponent,
                in TrailEffectData trailEffectData
        ) =>
            {
                if(groundInfo.HasComponent(trailEffectData.TargetEntity))
                {
                    var gInfo = groundInfo[trailEffectData.TargetEntity];
                    var pos = gInfo.GroundPosition;
                  
                    AddNewPosition(ref trailEffectViewComponent, ref trailEffectBuffer, pos, Vector3.up);
                }
            }).WithReadOnly(groundInfo).
            ScheduleParallel();
    }
   
    private static void AddNewPosition(
        ref TrailEffectViewComponent trailEffectViewComponent,
        ref DynamicBuffer<TrailEffectInfoData> trailEffectBuffer,
        Vector3 point, 
        Vector3 normal
    )
    {
        /*
        if (trailEffectViewComponent.LastPoint.Magnitude() != 0)
        {   
            Vector2 curPos = new Vector2(point.x, point.z);
            Vector2 prevPointPos = new Vector2(trailEffectViewComponent.LastPoint.x, trailEffectViewComponent.LastPoint.z);

            if (Vector2.Distance(curPos, prevPointPos) < trailEffectViewComponent.PointsDistance)
                return;
        }
        */
        float3 PlacePoint = point + normal * trailEffectViewComponent.HeightOffset;

        trailEffectViewComponent.LastPoint = PlacePoint;

        trailEffectBuffer.Add(new TrailEffectInfoData()
        {
            TrailPoint = trailEffectViewComponent.LastPoint
        });

        if (trailEffectBuffer.Length > trailEffectViewComponent.MaxTrailPoints)
            trailEffectBuffer.RemoveAt(0);

        //trailEffectViewComponent.UVPos += trailEffectViewComponent.UVInc;
        //if (trailEffectViewComponent.UVPos > 1000)
        //    trailEffectViewComponent.UVPos = 0;

        trailEffectViewComponent.MeshUpdated = true;
    }
}
