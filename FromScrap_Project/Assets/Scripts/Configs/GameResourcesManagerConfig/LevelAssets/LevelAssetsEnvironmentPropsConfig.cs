using System.Collections.Generic;
using Serializable.ResourcesManager;
using UnityEngine;

namespace Configs.GameResourcesManagerConfig.LevelAssets
{
    [CreateAssetMenu(fileName = "LevelAssetsEnvironmentPropsConfig", menuName = "Configs/ResourcesManager/Level/LevelAssetsEnvironmentPropsConfig", order = 1)]

    public class LevelAssetsEnvironmentPropsConfig : ScriptableObject
    {
        [SerializeField] public List<LevelAssetsEnvironmentPropAsset> EnvironmentPropAssetsList;
    }
}
