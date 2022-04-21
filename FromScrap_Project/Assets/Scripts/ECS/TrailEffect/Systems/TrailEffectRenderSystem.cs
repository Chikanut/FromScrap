using System.Collections;
using System.Collections.Generic;
using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

//[UpdateAfter(typeof(TrailEffectCalculateSystem))]
//[UpdateInGroup( typeof(PresentationSystemGroup) )]
    
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

        ComponentDataFromEntity<Translation> myTypeFromEntity = GetComponentDataFromEntity<Translation>(true);
        
        var ecb = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((
            Entity entity,
            ref TrailEffectViewComponent trailEffectViewComponent,
            ref DynamicBuffer<TrailEffectInfoData> trailEffectBuffer,
            ref DynamicBuffer<TrailEffectLastInfoData> trailEffectLastBuffer,
            in RenderMesh renderMesh
        ) =>
        {
            RenderTrailEffectPoints(
                ref trailEffectViewComponent,
                ref trailEffectBuffer,
                ref trailEffectLastBuffer,
                in renderMesh,
                fixedDeltaTime
            );
        }).WithoutBurst().Run();
    }
    
    private static void RenderTrailEffectPoints(
        ref TrailEffectViewComponent trailEffectViewComponent,
        ref DynamicBuffer<TrailEffectInfoData> trailEffectBuffer,
        ref DynamicBuffer<TrailEffectLastInfoData> trailEffectLastBuffer,
        in RenderMesh renderMesh,
        float deltaTime
    )
    {
        var LastPointData = trailEffectLastBuffer[0];
        
        if (!trailEffectViewComponent.MeshUpdated && !trailEffectViewComponent.FadeTrail)
            return;

        if (trailEffectViewComponent.FadeTrail)
            for (var i = 0; i < trailEffectBuffer.Length; i++)
            {
                if (trailEffectBuffer[i].Lifetime > 0f)
                {
                    trailEffectBuffer[i] = new TrailEffectInfoData()
                    { 
                        Point_Center = trailEffectBuffer[i].Point_Center,
                        Point1_Lt = trailEffectBuffer[i].Point1_Lt,
                        Point2_Rt = trailEffectBuffer[i].Point2_Rt,
                        UVPos1_Lt = trailEffectBuffer[i].UVPos1_Lt,
                        UVPos2_Rt = trailEffectBuffer[i].UVPos2_Rt,
                        Lifetime = trailEffectBuffer[i].Lifetime - deltaTime
                    };
                }
            }
      
        if (trailEffectBuffer.Length < 3)
            return;

        var verticies = new Vector3[trailEffectBuffer.Length * 2];
        var uv = new Vector2[trailEffectBuffer.Length * 2];
        var colors = new Color[trailEffectBuffer.Length * 2];
        var triangles = new int[trailEffectBuffer.Length * 6];
        var startCounter = 0f;
        var counter = 0;

        for (var i = 0; i < trailEffectBuffer.Length - 1; i++)
        {
            var startPoint = trailEffectBuffer[i];
            var endPoint = trailEffectBuffer[i + 1];

            float startFadeValue = 1f;
            if (trailEffectViewComponent.StartFadePoints>0)
             startFadeValue = Mathf.Clamp01(startCounter / trailEffectViewComponent.StartFadePoints);

            verticies[counter * 2] = startPoint.Point1_Lt;
                verticies[counter * 2 + 1] = startPoint.Point2_Rt;
                verticies[counter * 2 + 2] = endPoint.Point1_Lt;
                verticies[counter * 2 + 3] = endPoint.Point2_Rt;

                uv[counter * 2] = startPoint.UVPos1_Lt;
                uv[counter * 2 + 1] = startPoint.UVPos2_Rt;
                uv[counter * 2 + 2] = endPoint.UVPos1_Lt;
                uv[counter * 2 + 3] = endPoint.UVPos2_Rt;

                triangles[counter * 6 + 0] = counter * 2 + 2;
                triangles[counter * 6 + 1] = counter * 2 + 0;
                triangles[counter * 6 + 2] = counter * 2 + 1;

                triangles[counter * 6 + 3] = counter * 2 + 3;
                triangles[counter * 6 + 4] = counter * 2 + 2;
                triangles[counter * 6 + 5] = counter * 2 + 1;

                colors[i * 2].a = startPoint.Lifetime / trailEffectViewComponent.TrailLifetime * startFadeValue;
                colors[i * 2+1].a = startPoint.Lifetime / trailEffectViewComponent.TrailLifetime * startFadeValue;
                colors[i * 2+2].a = startPoint.Lifetime / trailEffectViewComponent.TrailLifetime * startFadeValue;
                colors[i * 2+3].a = startPoint.Lifetime / trailEffectViewComponent.TrailLifetime * startFadeValue;
              
                counter++;
                startCounter += 1f;
                
            if (startPoint.Lifetime <= 0f && endPoint.Lifetime <= 0f)
                startCounter = 0f;
        }

        var newMesh = new Mesh();
        
        newMesh.vertices = verticies;
        newMesh.triangles = triangles;
        
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();

        renderMesh.mesh = newMesh;
        
        
        renderMesh.mesh.vertices = verticies;
        renderMesh.mesh.triangles = triangles;
        //renderMesh.mesh.uv = uv;
        //renderMesh.mesh.colors = colors;

        renderMesh.mesh.RecalculateNormals();
        renderMesh.mesh.RecalculateTangents();
        //renderMesh.mesh.RecalculateBounds();
      
        trailEffectViewComponent.MeshUpdated = false;
    }
}
