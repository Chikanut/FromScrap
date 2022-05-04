using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainBaseComponent : IComponentData
    {
        public float3 TerrainTileSize { get; set; }
        public CollisionFilter CollisionFilter { get; set; }
    }
}
