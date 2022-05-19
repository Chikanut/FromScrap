using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.DynamicTerrainSystem
{
    [Serializable]
    public enum DynamicTerrainTileState
    {
        IsGenerated,
        IsReadyToGenerate,
        IsReadyToDestroy
    }
    
    [UpdateInGroup(typeof(DynamicTerrainSimulationGroup), OrderLast = true)]

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
            
            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
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
                            GenerateTerrainTile(ref entity, ref terrainTileBuffer, in dynamicTerrainBaseComponent, ecbs,entityInQueryIndex, i);
                            break;
                        case DynamicTerrainTileState.IsGenerated:
                            break;
                        case DynamicTerrainTileState.IsReadyToDestroy:
                            RemoveTerrainTile(ref terrainTileBuffer, ecbs, entityInQueryIndex, i);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }).ScheduleParallel(Dependency);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
        
        private static void GenerateTerrainTile(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            in DynamicTerrainBaseComponent terrainComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            int entityInQueryIndex,
            int index
        )
        {
            var tileData = terrainTileBuffer[index];
            var tileIndex = tileData.TileIndex;
            var tileEntity = ecbs.Instantiate(entityInQueryIndex, terrainComponent.TerrainTileEntity);
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
            ecbs.AddComponent(entityInQueryIndex, tileEntity, translation);
            ecbs.AddComponent(entityInQueryIndex, tileEntity, tileComponent);
            ecbs.AppendToBuffer(entityInQueryIndex, generatorEntity, new DynamicTerrainTileInfoData()
            {
                TileEntity = tileEntity,
                TileIndex = tileData.TileIndex,
                TileState = DynamicTerrainTileState.IsGenerated
            });
        }
        
        private static void RemoveTerrainTile(
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            EntityCommandBuffer.ParallelWriter ecbs,
            int entityInQueryIndex,
            int index
        )
        {
            var tileData = terrainTileBuffer[index];
            var tileEntity = tileData.TileEntity;
            
            ecbs.DestroyEntity(entityInQueryIndex, tileEntity);

            terrainTileBuffer.RemoveAtSwapBack(index);
        }
        
        private static float3 PositionFromTile(float2 tile, float3 tileSize, float3 terrainStartPos)
        {
            return new float3(tile.x * tileSize.x, terrainStartPos.y, tile.y * tileSize.z);
        }
    }
}