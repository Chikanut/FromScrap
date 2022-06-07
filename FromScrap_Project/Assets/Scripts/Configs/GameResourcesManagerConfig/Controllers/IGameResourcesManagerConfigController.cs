using Configs.GameResourcesManagerConfig.BaseResources;
using Serializable.ResourcesManager;
using UnityEngine;

namespace Configs.GameResourcesManagerConfig.Controllers
{
    public interface IGameResourcesManagerConfigController
    {
        public GameResourcesManagerConfigData GetGameResourcesData { get; }
        void SetInfo(BaseResourcesManagerConfig data);
    }
}
