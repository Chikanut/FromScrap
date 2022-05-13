using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainTileGeneratorSystem : SystemBase
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
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer
            ) =>
            {
                for (var i = 0; i < terrainTileBuffer.Length; i++)
                {
                    var tileInfoData = terrainTileBuffer[i];
                    
                    switch (tileInfoData.TileState)
                    {
                        case DynamicTerrainTileState.IsReadyToGenerate:
                            GenerateTerrainTile(ref entity, ref terrainTileBuffer, in dynamicTerrainBaseComponent, ecbs, i);
                            break;
                        case DynamicTerrainTileState.IsGenerated:
                            break;
                        case DynamicTerrainTileState.IsReadyToDestroy:
                            RemoveTerrainTile(ref terrainTileBuffer, ecbs, i);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
        
        private static void GenerateTerrainTile(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            in DynamicTerrainBaseComponent terrainComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            int index
        )
        {
            var tileData = terrainTileBuffer[index];
            var tileIndex = tileData.TileIndex;
            var tileEntity = ecbs.Instantiate(0, terrainComponent.TerrainTileEntity);
            var tileComponent = new DynamicTerrainTileComponent()
            {
                TerrainTileSize = terrainComponent.TerrainTileSize,
                CellSize = terrainComponent.CellSize,
                NoiseScale = terrainComponent.NoiseScale,
                VertexColorPower = terrainComponent.VertexColorPower,
                TileIndex = tileIndex,
                NormalsSmoothAngle = terrainComponent.NormalsSmoothAngle,
                UVMapScale = terrainComponent.UVMapScale,
                UVMapChannel = terrainComponent.UVMapChannel,
                IsVertexColorsEnabled = terrainComponent.IsVertexColorsEnabled,
                CollisionFilter = terrainComponent.CollisionFilter,
                IsUpdated = false
            };
            var translation = new Translation()
            {
                Value = PositionFromTile(tileIndex, terrainComponent.TerrainTileSize, terrainComponent.TerrainStartPosition)
            };
           
            terrainTileBuffer.RemoveAt(index);
            ecbs.AddComponent(0, tileEntity, translation);
            ecbs.AddComponent(0, tileEntity, tileComponent);
            ecbs.AppendToBuffer(0, generatorEntity, new DynamicTerrainTileInfoData()
            {
                TileEntity = tileEntity,
                TileIndex = tileData.TileIndex,
                TileState = DynamicTerrainTileState.IsGenerated
            });
        }
        
        private static void RemoveTerrainTile(
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            EntityCommandBuffer.ParallelWriter ecbs,
            int index
        )
        {
            var tileData = terrainTileBuffer[index];
            var tileEntity = tileData.TileEntity;
            
            ecbs.DestroyEntity(0, tileEntity);

            terrainTileBuffer.RemoveAt(index);
        }
        
        private static float3 PositionFromTile(float2 tile, float3 tileSize, float3 terrainStartPos)
        {
            return new float3(tile.x * tileSize.x, terrainStartPos.y, tile.y * tileSize.z);
        }
    }
}