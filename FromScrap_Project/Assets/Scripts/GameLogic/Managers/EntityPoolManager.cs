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

    public int _entitiesLibraryIndexes;

    private NativeHashMap<FixedString32Bytes, Entity> _entitiesLibrary = new NativeHashMap<FixedString32Bytes, Entity>(0, Allocator.Persistent);

    private bool _inited;
    
    private void OnEnable()
    {
        Init();
    }

    private void OnDestroy()
    {
        _poolBlobStore.Dispose();
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

        _entitiesLibraryIndexes = _entitiesLibrary.Capacity;
        if (!_entitiesLibrary.ContainsKey(templateName))
        {
            return GetObject(Resources.Load(templateName) as GameObject, setupCallback);
        }
        else
        {
            var entity = _entityManager.Instantiate(_entitiesLibrary[templateName]);

            setupCallback?.Invoke(entity, _entityManager);
        
            return entity;  
        }
    }

    public Entity GetObject(GameObject template, Action<Entity, EntityManager> setupCallback = null)
    {
        _entitiesLibraryIndexes = _entitiesLibrary.Capacity;
        var instanceID = template.name;

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
    
    public Entity GetObject(Entity template, Action<Entity, EntityManager> setupCallback = null)
    {
        var entity = _entityManager.Instantiate(template);

        setupCallback?.Invoke(entity, _entityManager);
        
        return entity;
    }
}
