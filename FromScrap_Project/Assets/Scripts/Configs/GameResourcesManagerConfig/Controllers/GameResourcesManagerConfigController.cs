using Configs.GameResourcesManagerConfig.BaseResources;
using Serializable.ResourcesManager;

namespace Configs.GameResourcesManagerConfig.Controllers
{
    public class GameResourcesManagerConfigController : IGameResourcesManagerConfigController
    {
        private GameResourcesManagerConfigData _model;
        
        public GameResourcesManagerConfigData GetGameResourcesData => _model;

        public virtual void SetInfo(BaseResourcesManagerConfig data)
        {
            _model = data.ResourcesManagerConfig;
        }
    }
}
