using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs.GameResourcesManagerConfig
{
    [CreateAssetMenu(fileName = "ResourcesManagerBaseConfig", menuName = "Configs/ResourcesManager/BaseConfig", order = 1)]

    public class BaseResourcesManagerConfig : ScriptableObject
    {
        [SerializeField] public AssetReference UIPrefabAsset;
    }
}
