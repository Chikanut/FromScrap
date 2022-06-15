using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [Serializable]
    public class PlayerUpgradesConfigData 
    {
        [Serializable]
        public class UpgradeLevelData
        {
            public GameObject Authoring;
            [Serializable]
            public class ItemInfo
            {
                public string ItemID;
                public int ItemsCount;
            }
            public List<ItemInfo> AddItems = new List<ItemInfo>();
            public int Cost;
            
            [Serializable]
            public class Description
            {
                public string DescriptionKey;
                public float[] Values;
            }

            public List<Description> Descriptions = new List<Description>();
        }

        [Serializable]
        public class UpgradeData
        {
            [Header("Info")]
            public string NameLocKey;
            public Sprite Icon;
            
            public List<UpgradeLevelData> UpgradesLevels = new List<UpgradeLevelData>();
        }
        
        [Serializable]
        public class UpgradesStepData
        {
            public int MinLevel;
            public List<UpgradeData> Upgrades = new List<UpgradeData>();
        }
        
        [Serializable]
        public class UpgradesDataCollection
        {
            public List<UpgradesStepData> Upgrades = new List<UpgradesStepData>();
        }
        
        public List<UpgradesDataCollection> UpgradesCollections = new List<UpgradesDataCollection>();

        public UpgradeLevelData GetUpgradeLevelData(int collectionID, int upgradeID, int upgradeLevel)
        {
            upgradeLevel -= 1;
            
            if (collectionID < 0 || collectionID >= UpgradesCollections.Count)
            {
                return null;
            }
            
            var upgradeInfo = GetUpgradeData(collectionID, upgradeID);
            if (upgradeInfo == null || upgradeInfo.UpgradesLevels.Count <= upgradeLevel)
            {
                return null;
            }
            
            return upgradeInfo.UpgradesLevels[upgradeLevel];
        }

        public UpgradeData GetUpgradeData(int collectionID, int upgradeID)
        {
            var upgradeCoordinate = GetUpgradeCoordinate(UpgradesCollections[collectionID], upgradeID);

            if (upgradeCoordinate != (-1, -1))
                return UpgradesCollections[collectionID].Upgrades[upgradeCoordinate.levelUpgradesID]
                    .Upgrades[upgradeCoordinate.upgradeIDinLevel];
            
            Debug.LogError("Can't find upgrade with id: " + upgradeID);
            return null;
        }

        (int levelUpgradesID, int upgradeIDinLevel) GetUpgradeCoordinate(UpgradesDataCollection collection, int upgradeID)
        {
            var levelUpgradesID = 0;
            var upgradeIDInLevel = 0;

            var currentUpgrade = 0;
            
            for (int i = 0; i < collection.Upgrades.Count; i++)
            {
                for (int j = 0; j < collection.Upgrades[i].Upgrades.Count; j++)
                {
                    if (currentUpgrade == upgradeID)
                    {
                        return (levelUpgradesID, upgradeIDInLevel);
                    }

                    upgradeIDInLevel++;
                    currentUpgrade++;
                }

                levelUpgradesID++;
                upgradeIDInLevel = 0;
            }
            
            return (-1, -1);
        }
    }
}
