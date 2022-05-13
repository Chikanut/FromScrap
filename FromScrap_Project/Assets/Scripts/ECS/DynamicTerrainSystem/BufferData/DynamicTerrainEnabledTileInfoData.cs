using Unity.Entities;
using Unity.Mathematics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainEnabledTileInfoData : IBufferElementData
    {
        public int2 TileIndex;
    }
}
