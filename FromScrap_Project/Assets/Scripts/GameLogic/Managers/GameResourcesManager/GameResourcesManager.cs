using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameLogic.Managers.GameResourcesManager
{
    public class GameResourcesManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private bool _loadDynamicTerrainOnStart = false;
        [SerializeField] private bool _loadLevelSpawnerOnStart = false;
        
        [Header("Components")] 
        [SerializeField] private AssetReference _baseResourcesConfig;
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
