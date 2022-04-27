using Unity.Entities;
using Unity.Mathematics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainBaseComponent : IComponentData
    {
        public float3 TerrainTileSize { get; set; }
    }
}
