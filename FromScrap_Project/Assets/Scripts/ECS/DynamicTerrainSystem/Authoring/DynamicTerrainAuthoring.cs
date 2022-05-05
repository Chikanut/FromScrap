using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    [Serializable]
    public class DynamicTerrainAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Terrain Settings")]
        [SerializeField]
        private float3 terrainTileSize = new float3(10f, 1f, 10f);
        public float cellSize = 1f;
        public float noiseScale = 2f;
        public float2 terrainStartPosition = new float2(0f, 0f);
        public float normalsSmoothAngle = 60f;
        public float uVMapScale = 8f;
        public int uVMapChannel = 0;
        public bool isVertexColorsEnabled = false;
        public float vertexColorPower = 2f;
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags collideWith;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }

            dstManager.AddComponentData(entity, new DynamicTerrainBaseComponent()
            {
                TerrainTileSize = terrainTileSize,
                CellSize = cellSize,
                NoiseScale = noiseScale,
                TerrainStartPosition = terrainStartPosition,
                NormalsSmoothAngle = normalsSmoothAngle,
                UVMapScale = uVMapScale,
                UVMapChannel = uVMapChannel,
                IsVertexColorsEnabled = isVertexColorsEnabled,
                VertexColorPower = vertexColorPower,
                CollisionFilter = new CollisionFilter()
                {
                    CollidesWith = collideWith.Value,
                    BelongsTo = belongsTo.Value
                }
            });
            
            var dynamicTerrainTileInfoData = dstManager.AddBuffer<DynamicTerrainTileInfoData>(entity);

            dynamicTerrainTileInfoData.Add(new DynamicTerrainTileInfoData()
            {
                TilePosition = terrainStartPosition,
                TileEntity = entity,
                TileState = DynamicTerrainTileState.IsReadyToGenerate
            });
        }
    }
}
