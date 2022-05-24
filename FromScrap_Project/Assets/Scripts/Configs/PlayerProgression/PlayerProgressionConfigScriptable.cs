using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "PlayerProgressionConfig", menuName = "Configs/PlayerProgressionConfig", order = 1)]
    public class PlayerProgressionConfigScriptable : ScriptableObject
    {
        public LevelsConfigScriptable PlayerLevels;
    }
}