using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "PlayerUpgradeConfig", menuName = "Configs/PlayerProgression/PlayerUpgradeConfig", order = 2)]
    public class PlayerUpgradeScriptable : ScriptableObject
    {
        public PlayerUpgradesConfigData.UpgradeData UpgradeData;
    }
}