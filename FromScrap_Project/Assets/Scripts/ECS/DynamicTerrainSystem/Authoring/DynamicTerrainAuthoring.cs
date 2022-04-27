using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    [Serializable]
    public class DynamicTerrainAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float3 terrainTileSize = new float3(10f, 1f, 10f);

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }

            dstManager.AddComponentData(entity, new DynamicTerrainBaseComponent()
            {
                TerrainTileSize = terrainTileSize
            });
            
            var dynamicTerrainTileInfoData = dstManager.AddBuffer<DynamicTerrainTileInfoData>(entity);

            dynamicTerrainTileInfoData.Add(new DynamicTerrainTileInfoData()
            {
                TileIndex = Vector2.zero,
                TileEntity = entity
            });
        }
    }
}
