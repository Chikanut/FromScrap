using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainTileGeneratorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer
            ) =>
            {
                for (var i = 0; i < terrainTileBuffer.Length; i++)
                {
                    var tileInfoData = terrainTileBuffer[i];
                    
                    switch (tileInfoData.TileState)
                    {
                        case DynamicTerrainTileState.IsReadyToGenerate:
                            GenerateTerrainTile(ref dynamicTerrainBaseComponent, ref terrainTileBuffer, i);
                            break;
                        case DynamicTerrainTileState.IsGenerated:

                            break;
                        case DynamicTerrainTileState.IsReadyToDestroy:

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }).ScheduleParallel();
        }

        private static void GenerateTerrainTile(
            ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            int index)
        {
            var tileData = terrainTileBuffer[index];
            var tilePos = tileData.TilePosition;
            
            //TODO: Spawn system
            EntityPoolManager.Instance.GetObject("DynamicTerrainTileTest", (entity, manager) =>
            {
                manager.SetComponentData(entity, new Translation()
                {
                    Value = new float3(tilePos.x, 10f, tilePos.y)
                });
            });
            
            terrainTileBuffer.RemoveAt(index);
            terrainTileBuffer.Add(new DynamicTerrainTileInfoData()
            {
                TileEntity = tileData.TileEntity,
                TilePosition = tileData.TilePosition,
                TileState = DynamicTerrainTileState.IsGenerated
            });
        }
    }
}