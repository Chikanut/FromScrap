using System;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainTileInfoData  : IBufferElementData
    {
        public float2 TilePosition;
        public Entity TileEntity;
        public DynamicTerrainTileState TileState;
    }

    [Serializable]
    public enum DynamicTerrainTileState
    {
        IsReadyToGenerate,
        IsGenerated,
        IsReadyToDestroy
    }
}
