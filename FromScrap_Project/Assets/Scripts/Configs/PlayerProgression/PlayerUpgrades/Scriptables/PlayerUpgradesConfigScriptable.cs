using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "PlayerUpgradesConfig", menuName = "Configs/PlayerProgression/PlayerUpgradesConfig", order = 4)]
    public class PlayerUpgradesConfigScriptable : ScriptableObject
    {
        public PlayerUpgradesConfigData UpgradeData {
            get
            {
                var upgradesData = new PlayerUpgradesConfigData();

                for (int i = 0; i < UpgradesCollections.Count; i++)
                {
                    upgradesData.UpgradesCollections.Add(UpgradesCollections[i].GetCollection());
                }

                return upgradesData;
            }
        }

        public List<PlayerUpgradesCollectionScriptable> UpgradesCollections =
            new List<PlayerUpgradesCollectionScriptable>();
    }
}
