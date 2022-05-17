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
            var currentTranslation = GetComponentDataFromEntity<Translation>(true);

            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
                ref DynamicBuffer<DynamicTerrainTrackInfoData> dynamicTerrainTrackBuffer
            ) =>
            {
                SetupDynamicTerrainTiles(
                    ref entity,
                    ref terrainTileBuffer,
                    ref dynamicTerrainTrackBuffer,
                    ref dynamicTerrainBaseComponent,
                    currentTranslation,
                    ecbs
                );
            }).WithReadOnly(currentTranslation).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        private static void SetupDynamicTerrainTiles(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            ref DynamicBuffer<DynamicTerrainTrackInfoData> dynamicTerrainTrackBuffer,
            ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
            ComponentDataFromEntity<Translation> currentTranslation,
            EntityCommandBuffer.ParallelWriter ecbs
        )
        {
            var isActive = false;

            for (var t = 0; t < dynamicTerrainTrackBuffer.Length; t++)
            {
                var isPlayer = dynamicTerrainTrackBuffer[t].IsPlayer;
                
                if(!isPlayer && !dynamicTerrainBaseComponent.AllowAdditionalElementsTracking)
                    continue;
                
                var curEntity = dynamicTerrainTrackBuffer[t].TrackEntity;
                var radiusToRender = dynamicTerrainBaseComponent.TrackingElementRadiusCount;

                if (isPlayer && currentTranslation.HasComponent(curEntity))
                {
                    radiusToRender = dynamicTerrainBaseComponent.PlayerRadiusCount;
                    isActive = true;
                }

                dynamicTerrainBaseComponent.IsActive = isActive;

                if (!isActive && !dynamicTerrainBaseComponent.AllowTrackingElementsAddTilesWithoutPlayer)
                    continue;

                if (!currentTranslation.HasComponent(curEntity))
                    continue;

                var playerPos = currentTranslation[curEntity].Value;
                var playerTile = TileFromPosition(playerPos, dynamicTerrainBaseComponent.TerrainTileSize);
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
            }
           
            for (var k = 0; k < dynamicTerrainTrackBuffer.Length; k++)
            {
                var curEntity = dynamicTerrainTrackBuffer[k].TrackEntity;

                if (!currentTranslation.HasComponent(curEntity))
                    dynamicTerrainTrackBuffer.RemoveAtSwapBack(k);
            }
          
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
