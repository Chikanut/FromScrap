using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs.GameResourcesManagerConfig.Configs
{
    [CreateAssetMenu(fileName = "CoreGameplayAssetsConfig", menuName = "Configs/ResourcesManager/CoreGameplayAssetsConfig", order = 1)]
    public class CoreGameplayAssetsConfig : ScriptableObject
    {
        [SerializeField] public AssetReference DynamicTerrainAsset;
    }
}
