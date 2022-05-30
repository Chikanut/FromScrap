using System;
using System.Collections;
using System.Collections.Generic;
using ECS.LevelSpawnerSystem;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TestSpawnLoadAssets : MonoBehaviour
{
    public AssetReference TestADPrefab;

    private BlobAssetStore _blobAssetStore;

    void Start()
    {
        AsyncOperationHandle<GameObject> goHandle = TestADPrefab.LoadAssetAsync<GameObject>();
        goHandle.Completed += operationHandle =>
        {
            //var MyPrefab = Instantiate(goHandle.Result);
            var MyPrefab = goHandle.Result;
            _blobAssetStore = new BlobAssetStore();
            GameObjectConversionSettings settings =
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
            var MyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(MyPrefab.gameObject, settings);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
          

          
            Entity entity = entityManager.CreateEntity();

            entityManager.AddComponentData ( entity, new TestSpawnElementId { } );
            
            entityManager.SetComponentData(entity, new TestSpawnElementId(){TargetEntity = MyEntity});
          
        };
    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }
}
