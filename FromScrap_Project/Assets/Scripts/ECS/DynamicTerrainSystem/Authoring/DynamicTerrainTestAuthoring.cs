using System;
using Unity.Entities;
using Unity.Mathematics;
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
                IsUpdated = isUpdated
            });
        }
    }
}
