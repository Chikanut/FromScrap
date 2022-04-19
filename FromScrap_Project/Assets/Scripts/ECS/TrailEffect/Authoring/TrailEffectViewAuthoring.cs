using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TrailEffectViewComponent : IComponentData
{
    public float PointsDistance { get; set; }
    public float HeightOffset { get; set; }
    public float BallSizeToTrailSize { get; set; }
    public float BallSpeedToTrailSize { get; set; }
    public float TrackWidthVariation { get; set; }
    public int MaxTrailPoints { get; set; }
    public float TrailLifetime { get; set; }
    public float UVInc { get; set; }

    public float3 LastPoint { get; set; }
    public float UVPos { get; set; }
    public bool MeshUpdated { get; set; }
}

[Serializable]
public sealed class TrailEffectViewAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Movement Settings")] public float pointsDistance = 0.5f;
    public float heightOffset = 0.1f;
    public float ballSizeToTrailSize = 1f;
    public float ballSpeedToTrailSize = 1f;
    public float trackWidthVariation = 0.2f;
    public int maxTrailPoints = 300;
    public float trailLifetime = 5f;
    public float uVInc = 0.5f;

    [Header("Current Parameters")] public float3 lastPoint;
    public float uVPos;
    public float3[] points = new float3[100];
    public bool meshUpdated = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }

        dstManager.AddComponentData(entity, new TrailEffectViewComponent()
        {
            PointsDistance = pointsDistance,
            HeightOffset = heightOffset,
            BallSizeToTrailSize = ballSizeToTrailSize,
            BallSpeedToTrailSize = ballSpeedToTrailSize,
            TrackWidthVariation = trackWidthVariation,
            MaxTrailPoints = maxTrailPoints,
            TrailLifetime = trailLifetime,
            UVInc = uVInc,

            LastPoint = lastPoint,
            UVPos = uVPos,
            MeshUpdated = meshUpdated
        });
    }
}