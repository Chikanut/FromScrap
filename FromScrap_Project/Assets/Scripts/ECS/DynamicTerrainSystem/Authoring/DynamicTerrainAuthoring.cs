using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Hybrid.Internal;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    //[Serializable]
    internal class DynamicTerrainAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [Header("Terrain Objects")]
        public GameObject terrainTilePrefab;
        [Header("Terrain Settings")]
        public float3 terrainTileSize = new float3(10f, 1f, 10f);
        public float cellSize = 1f;
        public float noiseScale = 2f;
        public float3 terrainStartPosition = new float3(0f, 0f, 0f);
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
                TerrainTileEntity = conversionSystem.GetPrimaryEntity(terrainTilePrefab),
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
            
            dstManager.AddBuffer<DynamicTerrainTileInfoData>(entity);
            dstManager.AddBuffer<DynamicTerrainEnabledTileInfoData>(entity);
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            GeneratedAuthoringComponentImplementation
                .AddReferencedPrefab(referencedPrefabs, terrainTilePrefab);
        }
    }
}
