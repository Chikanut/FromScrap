using Unity.Entities;
using Unity.Mathematics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainTileInfoData  : IBufferElementData
    {
        public float2 TileIndex;
        public Entity TileEntity;
    }
}
