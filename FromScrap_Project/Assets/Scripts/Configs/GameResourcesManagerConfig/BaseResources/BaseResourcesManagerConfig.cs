using Serializable.ResourcesManager;
using UnityEngine;

namespace Configs.GameResourcesManagerConfig.BaseResources
{
    [CreateAssetMenu(fileName = "ResourcesManagerBaseConfig", menuName = "Configs/ResourcesManager/BaseResourcesManagerConfig", order = 1)]

    public class BaseResourcesManagerConfig : ScriptableObject
    {
        public GameResourcesManagerConfigData ResourcesManagerConfig;
    }
}
