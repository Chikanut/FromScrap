using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainRemoveTileSystem : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecbs = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
           
            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
                ref DynamicBuffer<DynamicTerrainEnabledTileInfoData> terrainEnabledTileBuffer
            ) =>
            {
              
                RemoveTerrainTiles(
                    ref entity,
                    ref terrainTileBuffer,
                    ref terrainEnabledTileBuffer,
                    ecbs
                    );
                  
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }

        private static void RemoveTerrainTiles(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            ref DynamicBuffer<DynamicTerrainEnabledTileInfoData> terrainEnabledTileBuffer,
            EntityCommandBuffer.ParallelWriter ecbs 
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
              
                    /*
                     terrainTileBuffer.RemoveAt(i);
                     ecbs.AppendToBuffer(0, generatorEntity, new DynamicTerrainTileInfoData()
                     {
                         TileEntity = bufferTileData.TileEntity,
                         TileIndex = bufferTileData.TileIndex,
                         TileState = DynamicTerrainTileState.IsReadyToDestroy
                     });
                   */
                }
            }
            
            terrainEnabledTileBuffer.Clear();
        }
    }
}
