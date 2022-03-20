using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>() ?? new GameObject("GameManager").AddComponent<GameManager>();
            
            return _instance;
        }
    }
    
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    
    private BlobAssetStore _spawnerBlobStore;
    private EntityManager _entityManager;
    private Entity _playerEntityPrefab;
    
    void Start()
    {
        _spawnerBlobStore = new BlobAssetStore();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        if(playerPrefab != null)
            InitPlayer();
        
    }

    void InitPlayer()
    {
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _spawnerBlobStore);
        _playerEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(playerPrefab, settings);
        
        var playerEntity = _entityManager.Instantiate(_playerEntityPrefab);
            
        _entityManager.SetComponentData(playerEntity, new Translation { Value = new float3(24,0,24)});
        // _entityManager.SetComponentData(playerEntity, new MoveForward { Speed = 4 }); 
    }
}
