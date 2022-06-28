using Serializable.ResourcesManager;
using UnityEngine;

namespace Configs.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "GameplayLevelsConfig", menuName = "Configs/Gameplay/GameplayLevelsConfig", order = 1)]

    public class GameplayLevelsConfig : ScriptableObject
    {
        public GameLevels GameplayLevels;
    }
}
