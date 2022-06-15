using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using ShootCommon.GlobalStateMachine;

namespace Visartech.Progress
{
    [Serializable]
    public class Progress : ProgressBase<Progress>
    {

        [Serializable]
        public class PlayerData : Observable
        {
            public PlayerData()
            {
                Car = 0;
                Level = 0;
            }

            public int Car;
            public int Level;
            public int Experience;
            public int Scrap;
        }

        [Serializable]
        public class GameData
        {
            public bool SoundEnabled = true;
            public bool MusicEnabled = true;
        }

        [Serializable]
        public class DevelopmentData
        {
            public bool isTesting;
            public string testSceneName;
            public StateMachineTriggers testState;
        }

        [Serializable]
        public class StatisticsData
        {
            public bool isFirstPlay = false;
            public int KillsRecord;
            public int DamageRecord;
            public int LevelRecord;
            public int TimeRecord;
        }

        [Serializable]
        public class BackpackData
        {
            public class Item
            {
                public string ID;
                public int Count;
            }
            
            public List<Item> Items = new List<Item>();

            public void AddItem(string itemID, int addCount)
            {
                if(string.IsNullOrEmpty(itemID))
                    return;
                
                var getItem =
                    Items.FirstOrDefault(x => x.ID == itemID);

                if (getItem != null)
                {
                    getItem.Count += addCount;
                }
                else
                {
                    Items.Add(new Item()
                    {
                        ID = itemID,
                        Count = addCount,
                    });
                }
            }

            public Item GetItem(string itemID)
            {
                if (string.IsNullOrEmpty(itemID))
                {
                    Debug.LogError("GetItem: itemID is null or empty");
                    return null;
                }

                var getItem =
                    Items.FirstOrDefault(x => x.ID == itemID);

                if (getItem != null)
                {
                    return getItem;
                }

                var newItem = new Item()
                {
                    ID = itemID,
                    Count = 0,
                };

                Items.Add(newItem);

                return newItem;
            }
        }

        [Serializable]
        public class PlayerUpgradesData
        {
            public int PreviousLevel;

            [Serializable]
            public class PlayerUpgrade
            {
                public int CollectionID;
                public int UpgradeID;
                public int Level;
            }

            public List<PlayerUpgrade> Upgrades = new List<PlayerUpgrade>();

            public void SetUpgrade(int collectionID, int upgradeID, int level)
            {
                var getUpgrade =
                    Upgrades.FirstOrDefault(x => x.CollectionID == collectionID && x.UpgradeID == upgradeID);

                if (getUpgrade != null)
                {
                    getUpgrade.Level = level;
                }
                else
                {
                    Upgrades.Add(new PlayerUpgrade()
                    {
                        CollectionID = collectionID,
                        UpgradeID = upgradeID,
                        Level = level
                    });
                }
            }

            public PlayerUpgrade GetUpgrade(int collectionID, int upgradeID)
            {
                var getUpgrade =
                    Upgrades.FirstOrDefault(x => x.CollectionID == collectionID && x.UpgradeID == upgradeID);

                if (getUpgrade != null)
                {
                    return getUpgrade;
                }

                var newUpgrade = new PlayerUpgrade()
                {
                    CollectionID = collectionID,
                    UpgradeID = upgradeID,
                    Level = 0
                };

                Upgrades.Add(newUpgrade);

                return newUpgrade;
            }
        }

        private PlayerData _playerData = new PlayerData();
        private GameData _gameData = new GameData();
        private DevelopmentData _developmentData = new DevelopmentData();
        private StatisticsData _statisticsData = new StatisticsData();
        private PlayerUpgradesData _playerUpgradesData = new PlayerUpgradesData();
        private BackpackData _backpackData = new BackpackData();

        public static PlayerData Player
        {
            get => instance._playerData;
            set => instance._playerData = value;
        }

        public static GameData Game {
            get => instance._gameData;
            set => instance._gameData = value;
        }
        
        public static DevelopmentData Development {
            get => instance._developmentData;
            set => instance._developmentData = value;
        }
        
        public static StatisticsData Statistics {
            get => instance._statisticsData;
            set => instance._statisticsData = value;
        }

        public static PlayerUpgradesData PlayerUpgrades
        {
            get => instance._playerUpgradesData;
            set => instance._playerUpgradesData = value;
        }
        
        public static BackpackData Backpack
        {
            get => instance._backpackData;
            set => instance._backpackData = value;
        }

        public void Reset()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            ClearAllFields();
        }
    }
}