using Configs.GameResourcesManagerConfig.Configs;
using Serializable.ResourcesManager;

namespace Configs.GameResourcesManagerConfig.Controllers
{
    public interface IGameResourcesManagerConfigController
    {
        public GameResourcesManagerConfigData GetGameResourcesData { get; }
        void SetInfo(BaseResourcesManagerConfig data);
    }
}
