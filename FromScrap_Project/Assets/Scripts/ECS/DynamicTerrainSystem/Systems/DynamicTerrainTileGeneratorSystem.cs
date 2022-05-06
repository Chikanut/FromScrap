using System;
using Unity.Entities;
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
            in DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            int index
        )
        {
            var tileData = terrainTileBuffer[index];
            var tilePos = tileData.TilePosition;
            var tileEntity = ecbs.Instantiate(0, dynamicTerrainBaseComponent.TerrainTileEntity);
            var tileComponent = new DynamicTerrainTileComponent()
            {
                TerrainTileSize = dynamicTerrainBaseComponent.TerrainTileSize,
                CellSize = dynamicTerrainBaseComponent.CellSize,
                NoiseScale = dynamicTerrainBaseComponent.NoiseScale,
                VertexColorPower = dynamicTerrainBaseComponent.VertexColorPower,
                TilePosition = tilePos,
                NormalsSmoothAngle = dynamicTerrainBaseComponent.NormalsSmoothAngle,
                UVMapScale = dynamicTerrainBaseComponent.UVMapScale,
                UVMapChannel = dynamicTerrainBaseComponent.UVMapChannel,
                IsVertexColorsEnabled = dynamicTerrainBaseComponent.IsVertexColorsEnabled,
                CollisionFilter = dynamicTerrainBaseComponent.CollisionFilter,
                IsUpdated = false
            };
            var translation = new Translation()
            {
                Value = tilePos
            };
           
            terrainTileBuffer.RemoveAt(index);
            ecbs.AddComponent(0, tileEntity, translation);
            ecbs.AddComponent(0, tileEntity, tileComponent);
            ecbs.AppendToBuffer(0, generatorEntity, new DynamicTerrainTileInfoData()
            {
                TileEntity = tileEntity,
                TilePosition = tileData.TilePosition,
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
    }
}