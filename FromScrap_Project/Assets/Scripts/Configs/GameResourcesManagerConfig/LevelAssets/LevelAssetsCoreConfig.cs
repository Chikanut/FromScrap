using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs.GameResourcesManagerConfig.LevelAssets
{
    [CreateAssetMenu(fileName = "LevelAssetsCoreConfig", menuName = "Configs/ResourcesManager/Level/LevelAssetsCoreConfig", order = 1)]

    public class LevelAssetsCoreConfig : ScriptableObject
    {
        [SerializeField] public AssetReference EnvoronmetPropsConfig;
    }
}
