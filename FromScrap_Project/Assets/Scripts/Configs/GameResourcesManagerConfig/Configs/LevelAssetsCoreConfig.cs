using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs.GameResourcesManagerConfig.Configs
{
    [CreateAssetMenu(fileName = "LevelAssetsCoreConfig", menuName = "Configs/ResourcesManager/LevelAssetsCoreConfig", order = 1)]

    public class LevelAssetsCoreConfig : ScriptableObject
    {
        [SerializeField] public AssetReference EnvoronmetPropsConfig;
    }
}
