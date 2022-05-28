using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "PlayerProgressionConfig", menuName = "Configs/PlayerProgression/PlayerProgressionConfig", order = 1)]
    public class PlayerProgressionConfigScriptable : ScriptableObject
    {
        public LevelsConfigScriptable PlayerLevels;
        public PlayerUpgradesConfigScriptable PlayerUpgrades;
    }
}