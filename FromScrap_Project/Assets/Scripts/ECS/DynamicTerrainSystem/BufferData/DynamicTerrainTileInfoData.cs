using System;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainTileInfoData : IBufferElementData
    {
        public int2 TileIndex;
        public Entity TileEntity;
        public DynamicTerrainTileState TileState;
    }

    [Serializable]
    public enum DynamicTerrainTileState
    {
        IsGenerated,
        IsReadyToGenerate,
        IsReadyToDestroy
    }
}
