using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;
using Collider = Unity.Physics.Collider;
using Material = UnityEngine.Material;
using Random = UnityEngine.Random;

public class TestObjectsECS : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _objectsCount;
    [SerializeField] private float3 _spawnRectPos;
    [SerializeField] private float2 _spawnRect;
    [SerializeField] private float2 _movementSpeedRange;
    [SerializeField] private float2 _rotationSpeedRange;
    [SerializeField] private bool _physicsTest;
    [SerializeField] private GameObject _physicsPrefab;

    [Header("Resources")]
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;

    public void SpawnObjects(int count)
    {
        _objectsCount = count;
        
        if (_physicsTest)
        {
            SpawnPhysicsEntities();
        }
        else
        {
            SpawnRenderEntities();
        }

    }

    void SpawnPhysicsEntities()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings =
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

        var physicsEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(_physicsPrefab, settings);
        
        for (int i = 0; i < _objectsCount; i++)
        {
            Entity newEntity = entityManager.Instantiate(physicsEntity);
            
            entityManager.SetComponentData(newEntity, new Translation()
            {
                Value = new float3(_spawnRectPos.x + Random.Range(-_spawnRect.x/2,_spawnRect.x/2),_spawnRectPos.y+Random.Range(-10,25),_spawnRectPos.z +Random.Range(-_spawnRect.y/2,_spawnRect.y/2))
            });
        }
    }

    void SpawnRenderEntities()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype meshTest = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(LocalToWorld)
        );
        
        NativeArray<Entity> entityArray = new NativeArray<Entity>(_objectsCount, Allocator.Temp);
        entityManager.CreateEntity(meshTest, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            // entityManager.AddSharedComponentData(entity, new PhysicsWorldIndex(){Value = 0});
            entityManager.SetComponentData(entity, new Translation()
            {
                Value = new float3(_spawnRectPos.x + Random.Range(-_spawnRect.x/2,_spawnRect.x/2),_spawnRectPos.y,_spawnRectPos.z +Random.Range(-_spawnRect.y/2,_spawnRect.y/2))
            });
            
            entityManager.AddComponentData(entity, new MovementComponent()
            {
                MovementSpeed = Random.Range(_movementSpeedRange.x, _movementSpeedRange.y),
                Limits = new float2(_spawnRectPos.z - _spawnRect.y/2,_spawnRectPos.z + _spawnRect.y/2)
            });
            entityManager.AddComponentData(entity, new RotateComponent()
            {
                RotationSpeed = Random.Range(_rotationSpeedRange.x, _rotationSpeedRange.y)
            });
            
            
            var desc = new RenderMeshDescription(
                _mesh,
                _material,
                shadowCastingMode: ShadowCastingMode.On,
                receiveShadows: true);

            RenderMeshUtility.AddComponents(
                entity,
                entityManager,
                desc);
        }
        
        entityArray.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(_spawnRectPos, new Vector3(_spawnRect.x, 0, _spawnRect.y));
    }
}
