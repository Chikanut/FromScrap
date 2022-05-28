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
    }
}
