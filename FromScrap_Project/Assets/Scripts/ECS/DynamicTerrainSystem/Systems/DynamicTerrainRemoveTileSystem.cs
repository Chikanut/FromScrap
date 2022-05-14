using Unity.Entities;

namespace ECS.DynamicTerrainSystem
{
    [UpdateInGroup(typeof(DynamicTerrainSimulationGroup), OrderFirst = true)]
    [UpdateAfter(typeof(DynamicTerrainAddTileSystem))]
    
    public partial class DynamicTerrainRemoveTileSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
                ref DynamicBuffer<DynamicTerrainEnabledTileInfoData> terrainEnabledTileBuffer
            ) =>
            {
                RemoveTerrainTiles(ref terrainTileBuffer, ref terrainEnabledTileBuffer);
            }).ScheduleParallel();
        }

        private static void RemoveTerrainTiles(
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            ref DynamicBuffer<DynamicTerrainEnabledTileInfoData> terrainEnabledTileBuffer 
        )
        {
            for (var i = 0; i < terrainTileBuffer.Length; i++)
            {
                var isExist = false;
                var bufferTileData = terrainTileBuffer[i];
                var bufferTileIndex = bufferTileData.TileIndex;
                
                for (var j = 0; j < terrainEnabledTileBuffer.Length; j++)
                {
                    var enabledTileIndex = terrainEnabledTileBuffer[j].TileIndex;

                    if (enabledTileIndex.x == bufferTileIndex.x && enabledTileIndex.y == bufferTileIndex.y)
                        isExist = true;
                }

                if (!isExist)
                {
                    terrainTileBuffer[i] = new DynamicTerrainTileInfoData()
                    {
                        TileEntity = bufferTileData.TileEntity,
                        TileIndex = bufferTileData.TileIndex,
                        TileState = DynamicTerrainTileState.IsReadyToDestroy
                    };
                }
            }
            
            terrainEnabledTileBuffer.Clear();
        }
    }
}
