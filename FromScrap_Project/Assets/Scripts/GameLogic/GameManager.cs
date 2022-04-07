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
    
    void Start()
    {
        if(playerPrefab != null)
            InitPlayer();
    }

    void InitPlayer()
    {
       EntityPoolManager.Instance.GetObject(playerPrefab, (entity, manager) =>
       {
           manager.SetComponentData(entity, new Translation()
           {
               Value = new float3(0,2,0)
           });
       });
    }
}
