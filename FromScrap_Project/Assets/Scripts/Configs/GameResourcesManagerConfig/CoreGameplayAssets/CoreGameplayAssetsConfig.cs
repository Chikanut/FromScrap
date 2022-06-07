using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs.GameResourcesManagerConfig.CoreGameplayAssets
{
    [CreateAssetMenu(fileName = "CoreGameplayAssetsConfig", menuName = "Configs/ResourcesManager/CoreGameplay/CoreGameplayAssetsConfig", order = 1)]
    public class CoreGameplayAssetsConfig : ScriptableObject
    {
        [SerializeField] public AssetReference DynamicTerrainAsset;
    }
}
