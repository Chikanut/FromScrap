using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainBaseComponent : IComponentData
    {
        public Entity TerrainTileEntity;
        public float3 TerrainTileSize;
        public float CellSize;
        public int PlayerRadiusCount;
        public bool AllowAdditionalElementsTracking;
        public int TrackingElementRadiusCount;
        public float NoiseScale;
        public float3 TerrainStartPosition;
        public float NormalsSmoothAngle;
        public float UVMapScale;
        public int UVMapChannel;
        public bool IsVertexColorsEnabled;
        public float VertexColorPower;
        public CollisionFilter CollisionFilter;
        public bool IsActive;
        public bool AllowTrackingElementsAddTilesWithoutPlayer;
    }
}
