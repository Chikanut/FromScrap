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
                ref DynamicBuffer<TrailEffectLastInfoData> trailEffectLastBuffer,
                ref TrailEffectViewComponent trailEffectViewComponent,
                in TrailEffectData trailEffectData
        ) =>
            {
                if(groundInfo.HasComponent(trailEffectData.TargetEntity))
                {
                    var gInfo = groundInfo[trailEffectData.TargetEntity];
                    var pos = gInfo.GroundPosition;
                  
                    AddNewPosition(ref trailEffectViewComponent,
                        ref trailEffectBuffer,
                        ref trailEffectLastBuffer,
                        pos,
                        new float3(0f, 1f, 0f), 
                        new float3(1f, 0f, 0f),
                        1f, 
                        1f
                        );
                }
            }).WithReadOnly(groundInfo).
            ScheduleParallel();
    }
   
    private static void AddNewPosition(
        ref TrailEffectViewComponent trailEffectViewComponent,
        ref DynamicBuffer<TrailEffectInfoData> trailEffectBuffer,
        ref DynamicBuffer<TrailEffectLastInfoData> trailEffectLastBuffer,
        float3 point, 
        float3 normal, 
        float3 forward,
        float speed,
        float size
    )
    {
        var LastPoint = trailEffectLastBuffer[0];
        
        if (LastPoint.IsActive)
        {   
            var curPos = new float2(point.x, point.z);
            var prevPointPos = new float2(LastPoint.Point_Center.x, LastPoint.Point_Center.z);

            if (Vector2.Distance(curPos, prevPointPos) < trailEffectViewComponent.PointsDistance)    
                return;
        }

        var placePoint = point + normal * trailEffectViewComponent.HeightOffset;
        var hitCross = !LastPoint.IsActive ? Vector3.Cross(-forward, normal).normalized : Vector3.Cross(LastPoint.Point_Center - placePoint, normal).normalized;
      
        var Point1_Lt = placePoint 
                        + new float3(hitCross.x, hitCross.y, hitCross.z) 
                        * trailEffectViewComponent.BallSizeToTrailSize 
                        * size 
                        * trailEffectViewComponent.BallSpeedToTrailSize 
                        * speed 
                        * (1f 
                           //+ UnityEngine.Random.Range(-trailEffectViewComponent.TrackWidthVariation, trailEffectViewComponent.TrackWidthVariation)
                           );
        var Point2_Rt = placePoint 
                        - new float3(hitCross.x, hitCross.y, hitCross.z) 
                        * trailEffectViewComponent.BallSizeToTrailSize 
                        * size 
                        * trailEffectViewComponent.BallSpeedToTrailSize 
                        * speed 
                        * (1f
                           //+ UnityEngine.Random.Range(-trailEffectViewComponent.TrackWidthVariation, trailEffectViewComponent.TrackWidthVariation)
                           );

        var lifetime = trailEffectViewComponent.TrailLifetime;        
       
        if (!LastPoint.IsActive)
            lifetime = 0f;

        trailEffectLastBuffer[0] = new TrailEffectLastInfoData()
        {
            Point_Center = placePoint,
            Point1_Lt = Point1_Lt,
            Point2_Rt = Point2_Rt,
            UVPos1_Lt = new Vector2(trailEffectViewComponent.UVPos, 0),
            UVPos2_Rt = new Vector2(trailEffectViewComponent.UVPos, 1),
            Lifetime = lifetime,
            IsActive = true
        };

        var newPoint = new TrailEffectInfoData()
        {
            Point_Center =  LastPoint.Point_Center,
            Point1_Lt = LastPoint.Point1_Lt,
            Point2_Rt = LastPoint.Point2_Rt,
            UVPos1_Lt = LastPoint.UVPos1_Lt,
            UVPos2_Rt = LastPoint.UVPos2_Rt,
            Lifetime = LastPoint.Lifetime
        };

        trailEffectBuffer.Add(newPoint);
        
        if (trailEffectBuffer.Length > trailEffectViewComponent.MaxTrailPoints)
            trailEffectBuffer.RemoveAt(0);

        trailEffectViewComponent.UVPos += trailEffectViewComponent.UVInc;
        if (trailEffectViewComponent.UVPos > 1000)
            trailEffectViewComponent.UVPos = 0;

        trailEffectViewComponent.MeshUpdated = true;
    }
}
