using Configs.Gameplay.Configs;

namespace Configs.Gameplay.Controllers
{
    public class GameplayLevelsConfigController : IGameplayLevelsConfigController
    {
        private GameplayLevelsConfig _model;
        
        public GameplayLevelsConfig GetGameplayLevelsData => _model;

        public virtual void SetInfo(GameplayLevelsConfig data)
        {
            _model = data;
        }
    }
}
