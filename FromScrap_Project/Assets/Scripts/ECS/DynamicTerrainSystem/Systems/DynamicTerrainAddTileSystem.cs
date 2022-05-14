using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    [UpdateInGroup(typeof(DynamicTerrainSimulationGroup), OrderFirst = true)]
    [UpdateBefore(typeof(DynamicTerrainRemoveTileSystem))]
    
    public partial class DynamicTerrainAddTileSystem : SystemBase
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
            var playerPosition = float3.zero;

            Entities.ForEach((
                Entity entity,
                in CarIDComponent carIDComponent,
                in Translation translation
            ) =>
            {
                playerPosition = translation.Value;
            }).Run();

            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
                ref DynamicBuffer<DynamicTerrainEnabledTileInfoData> terrainEnabledTileBuffer
            ) =>
            {
                
                SetupDynamicTerrainTiles(
                    ref entity,
                    ref terrainTileBuffer,
                    ref terrainEnabledTileBuffer,
                    in dynamicTerrainBaseComponent,
                    ecbs,
                    playerPosition
                    );
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void SetupDynamicTerrainTiles(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            ref DynamicBuffer<DynamicTerrainEnabledTileInfoData> terrainEnabledTileBuffer,
            in DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            float3 playerPos
        )
        {
           
            var radiusToRender = dynamicTerrainBaseComponent.TerrainTilesRadiusCount;
            var playerTile = TileFromPosition(playerPos, dynamicTerrainBaseComponent.TerrainTileSize);
            //var centerTiles = new NativeArray<int2>(10, Allocator.Temp);
            //centerTiles[0] = playerTile;
            
            //terrainEnabledTileBuffer.Clear();

            //for (var t = 0; t < centerTiles.Length; t++)
            //{
                var tile = playerTile;
                var isPlayerTile = tile.x == playerTile.x && tile.y == playerTile.y;
                var radius = isPlayerTile ? radiusToRender : 1;
                for (var i = -radius; i <= radius; i++)
                for (var j = -radius; j <= radius; j++)
                {
                    var tileIndex = new int2(tile.x + i, tile.y + j);

                    ecbs.AppendToBuffer(0, generatorEntity, new DynamicTerrainEnabledTileInfoData()
                    {
                       TileIndex = tileIndex
                    });
                    
                    AddTileData(ref generatorEntity, ref terrainTileBuffer, ecbs, tileIndex);
                }
            //}
        }

        private static void AddTileData(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            EntityCommandBuffer.ParallelWriter ecbs,
            int2 tileIndex
        )
        {
            var exist = false;

            for (var index = 0; index < terrainTileBuffer.Length; index++)
            {
                var tileData = terrainTileBuffer[index];
                var currentTileIndex = tileData.TileIndex;

                if (tileIndex.x == currentTileIndex.x && tileIndex.y == currentTileIndex.y)
                    exist = true;
            }

            if (!exist)
            {
                ecbs.AppendToBuffer(0, generatorEntity, new DynamicTerrainTileInfoData()
                {
                    TileIndex = tileIndex,
                    TileState = DynamicTerrainTileState.IsReadyToGenerate
                });
            }
        }

        private static int2 TileFromPosition(float3 position, float3 terrainSize)
        {
            return new int2(Mathf.FloorToInt(position.x / terrainSize.x + .5f), Mathf.FloorToInt(position.z / terrainSize.z + .5f));
        }
    }
}
