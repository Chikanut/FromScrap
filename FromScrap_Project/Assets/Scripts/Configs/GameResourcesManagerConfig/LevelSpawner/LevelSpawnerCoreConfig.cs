using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs.GameResourcesManagerConfig.LevelSpawner
{
    [CreateAssetMenu(fileName = "LevelSpawnerCoreConfig", menuName = "Configs/ResourcesManager/LevelSpawner/CoreConfig", order = 1)]

    public class LevelSpawnerCoreConfig : ScriptableObject
    {
        [SerializeField] public AssetReference EnvoronmetPropsConfig;
    }
}
