using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainBaseComponent : IComponentData
    {
        public float3 TerrainTileSize { get; set; }
        public float CellSize { get; set; }
        public float NoiseScale { get; set; }
        public float2 TerrainStartPosition { get; set; }
        public float NormalsSmoothAngle { get; set; }
        public float UVMapScale { get; set; }
        public int UVMapChannel { get; set; }
        public bool IsVertexColorsEnabled { get; set; }
        public float VertexColorPower { get; set; }
        public CollisionFilter CollisionFilter { get; set; }
    }
}
