using System.Collections.Generic;
using Serializable.ResourcesManager;
using UnityEngine;

namespace Configs.GameResourcesManagerConfig.Configs
{
    [CreateAssetMenu(fileName = "LevelAssetsEnvironmentPropsConfig", menuName = "Configs/ResourcesManager/LevelAssetsEnvironmentPropsConfig", order = 1)]

    public class LevelAssetsEnvironmentPropsConfig : ScriptableObject
    {
        [SerializeField] public List<LevelAssetsEnvironmentPropAsset> EnvironmentPropAssetsList;
    }
}
