using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EntityPoolManager : MonoBehaviour
{
    private static EntityPoolManager _instance;

    public static EntityPoolManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<EntityPoolManager>() ?? new GameObject("EntityPoolManager").AddComponent<EntityPoolManager>();
                
            _instance.Init();

            return _instance;
        }
    }

    private BlobAssetStore _poolBlobStore;
    private GameObjectConversionSettings _conversionSettings;
    private EntityManager _entityManager;

    private NativeHashMap<int, Entity> _entitiesLibrary = new NativeHashMap<int, Entity>(0, Allocator.Persistent);

    private bool _inited;
    
    private void OnEnable()
    {
        Init();
    }

    private void OnDestroy()
    {
        _entitiesLibrary.Dispose();
    }

    void Init()
    {
        if(_inited) return;
        
        _poolBlobStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _conversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _poolBlobStore);

        _inited = true;
    }

    public Entity GetObject(string templateName, Action<Entity, EntityManager> setupCallback = null)
    {
        return GetObject(Resources.Load(templateName) as GameObject, setupCallback);
    }

    public Entity GetObject(GameObject template, Action<Entity, EntityManager> setupCallback = null)
    {
        var instanceID = template.GetInstanceID();

        if (!_entitiesLibrary.ContainsKey(instanceID))
        {
            var entityTemplate = GameObjectConversionUtility.ConvertGameObjectHierarchy(template, _conversionSettings);

            _entitiesLibrary.Capacity++;
            _entitiesLibrary.Add(instanceID, entityTemplate);
        }
        
        var entity = _entityManager.Instantiate(_entitiesLibrary[instanceID]);

        setupCallback?.Invoke(entity, _entityManager);
        
        return entity;
    }
}
