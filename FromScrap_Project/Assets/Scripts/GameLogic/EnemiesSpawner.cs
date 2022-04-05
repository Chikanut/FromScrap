using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesSpawner : MonoBehaviour
{
    private static EnemiesSpawner _instance;

    public static EnemiesSpawner Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EnemiesSpawner>() ?? new GameObject("EnemySpawner").AddComponent<EnemiesSpawner>();
            
            return _instance;
        }
    }
    
    [Header("Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;
    
    [Header("Spawn Info")]
    [SerializeField] private int spawnCount = 15;
    [SerializeField] private float spawnRadius = 8;
    [SerializeField] private int difficultyBonus = 1;
    [SerializeField] private float spawnTimeStep = 15;

    [Header("Movement Info")] 
    [SerializeField] private float minSpeed = 4;
    [SerializeField] private float maxSpeed = 6;

    private BlobAssetStore _spawnerBlobStore;
    private EntityManager _entityManager;
    private Entity[] _enemyEntityPrefabs;

    void Start()
    {
        _spawnerBlobStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _spawnerBlobStore);
        _enemyEntityPrefabs = new Entity[enemyPrefabs.Length];
        for (int i = 0; i < _enemyEntityPrefabs.Length; i++)
        {
            _enemyEntityPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefabs[i], settings);
        } 

        StartGame();
    }

    public void StartGame()
    {
        SpawnWave();
    }

    private void SpawnWave()
    {
        var enemyArray = new NativeArray<Entity>(spawnCount, Allocator.Temp);
        
        for (int i = 0; i < enemyArray.Length; i++)
        {
            enemyArray[i] =
                _entityManager.Instantiate(_enemyEntityPrefabs[Random.Range(0, _enemyEntityPrefabs.Length)]);

            _entityManager.SetComponentData(enemyArray[i],
                new Translation
                {
                    Value = RandomPointOnCircle(new float3(0, Random.Range(5, 25), 0),
                        spawnRadius + Random.Range(-spawnRadius * 0.25f, spawnRadius * 0.25f))
                });
        }
        
        enemyArray.Dispose();
        
        spawnCount += difficultyBonus;
    }

    float3 RandomPointOnCircle(float3 center, float radius)
    {
        var angle = 2.0f * math.PI * UnityEngine.Random.Range(0f, 360f);

        return new float3(center.x + radius * math.cos(angle), center.y, center.z + radius * math.sin(angle));
    }
}
