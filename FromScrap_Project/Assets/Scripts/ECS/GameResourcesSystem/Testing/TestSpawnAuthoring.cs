using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Hybrid.Internal;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ECS.LevelSpawnerSystem
{
    internal class TestSpawnAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [Header("Terrain Objects")] 
        public GameObject TestSpawnPrefab;
        [Header("Terrain Settings")] public int EntityCount = 1000;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!enabled)
            {
                return;
            }
            
            var testSpawnEntity = conversionSystem.GetPrimaryEntity(TestSpawnPrefab);
            dstManager.AddComponentData(entity, new TestSpawnComponent()
            {
                SpawnEntity = testSpawnEntity,
                EntityCount = EntityCount,
                EnableSpawn = false,
                EnableLoad = false
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            GeneratedAuthoringComponentImplementation.AddReferencedPrefab(referencedPrefabs, TestSpawnPrefab);
        }
    }
}
