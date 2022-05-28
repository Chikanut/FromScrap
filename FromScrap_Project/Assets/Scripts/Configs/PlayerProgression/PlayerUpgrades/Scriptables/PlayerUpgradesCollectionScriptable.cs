using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "PlayerUpgradesCollectionConfig", menuName = "Configs/PlayerProgression/PlayerUpgradesCollectionConfig", order = 3)]
    public class PlayerUpgradesCollectionScriptable : ScriptableObject
    {
        [Serializable]
        public class UpgradesStepData
        {
            public int MinLevel;
            public List<PlayerUpgradeScriptable> Upgrades = new List<PlayerUpgradeScriptable>();

            public PlayerUpgradesConfigData.UpgradesStepData GetUpgradesStepData()
            {
                var result = new PlayerUpgradesConfigData.UpgradesStepData();

                result.MinLevel = MinLevel;

                for (int i = 0; i < Upgrades.Count; i++)
                {
                    result.Upgrades.Add(Upgrades[i].UpgradeData);
                }

                return result;
            }
        }
        
        public List<UpgradesStepData> Upgrades = new List<UpgradesStepData>();

        public PlayerUpgradesConfigData.UpgradesDataCollection GetCollection()
        {
            var result = new PlayerUpgradesConfigData.UpgradesDataCollection();
            foreach (var step in Upgrades)
            {
                result.Upgrades.Add(step.GetUpgradesStepData());
            }
            return result;
        }
    }
}