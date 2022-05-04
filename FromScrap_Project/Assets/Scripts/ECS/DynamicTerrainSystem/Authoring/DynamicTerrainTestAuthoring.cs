using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    public class DynamicTerrainTestAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Terrain Settings")] 
        public float3 terrainSize = new float3(10f, 1f, 10f);
        public float cellSize = 10f;
        public float noiseScale = 2f;
        public float gradient = 2f;
        public float2 noiseOffset = new float2(10f, 10f);
        public float normalsSmoothAngle = 60f;
        public float uVMapScale = 8f;
        public int uVMapChannel = 0;
        public bool isVertexColorsEnabled = false;
        public PhysicsCategoryTags belongsTo;
        public PhysicsCategoryTags collideWith;
        public bool isUpdated = false;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }

            dstManager.AddComponentData(entity, new DynamicTerrainTileComponent()
            {
                TerrainSize = terrainSize,
                CellSize = cellSize,
                NoiseScale = noiseScale,
                Gradient = gradient,
                NoiseOffset = noiseOffset,
                IsUpdated = isUpdated,
                NormalsSmoothAngle = normalsSmoothAngle,
                UVMapScale = uVMapScale,
                UVMapChannel = uVMapChannel,
                IsVertexColorsEnabled = isVertexColorsEnabled,
                CollisionFilter = new CollisionFilter()
                {
                    CollidesWith = collideWith.Value,
                    BelongsTo = belongsTo.Value
                }
            });
        }
    }
}
