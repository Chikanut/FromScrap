using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainTileComponent: IComponentData
    {
        public float3 TerrainSize;
        public float CellSize;
        public float NoiseScale;
        public float VertexColorPower;
        public float2 TilePosition;
        public float NormalsSmoothAngle;
        public float UVMapScale;
        public int UVMapChannel;
        public bool IsVertexColorsEnabled;
        public CollisionFilter CollisionFilter;
        public bool IsUpdated;
    }
}
