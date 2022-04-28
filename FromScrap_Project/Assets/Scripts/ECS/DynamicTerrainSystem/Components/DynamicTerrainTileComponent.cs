using Unity.Entities;
using Unity.Mathematics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainTileComponent: IComponentData
    {
        public float3 TerrainSize;
        public float CellSize;
        public float NoiseScale;
        public float Gradient;
        public float2 NoiseOffset;
        public bool IsUpdated;
    }
}
