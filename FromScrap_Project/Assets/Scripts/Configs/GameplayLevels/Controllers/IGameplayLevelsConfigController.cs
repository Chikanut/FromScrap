using Configs.Gameplay.Configs;

namespace Configs.Gameplay.Controllers
{
    public interface IGameplayLevelsConfigController
    {
        public GameplayLevelsConfig GetGameplayLevelsData { get; }
        void SetInfo(GameplayLevelsConfig data);
    }
}
