using System;
using System.Collections.Generic;
using FromScrap.Tools;
using Packages.Common.Storage.Config;
using UnityEngine;
using Visartech.Progress;

namespace UI.Screens.MainMenu.Tabs.Upgrades.Elements
{
    public class UpgradesCollectionPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<UpgradesLevelPanel> _defaultUpgradesLevels;
        [SerializeField] private ObjectPool<UpgradesLevelPanel> _upgradeLevelsPool;

        private Action<int, int> _onIconPressed;
        private int _collectionID;
        
        public void Init(int currentLevel, int currentScrap, PlayerUpgradesConfigData.UpgradesDataCollection collectionInfo, int collectionID, Action<int,int> onIconPressed)
        {
            _onIconPressed = onIconPressed;
            _collectionID = collectionID;
            
            ClearAll();

            int upgradesStep = 0;
            for (int i = 0; i < collectionInfo.Upgrades.Count; i++)
            {
                InitUpgradeLevel(
                    i < _defaultUpgradesLevels.Count ? _defaultUpgradesLevels[i] : _upgradeLevelsPool.GetNextObject(),
                    collectionInfo.Upgrades[i], currentLevel, currentScrap, upgradesStep);

                upgradesStep += collectionInfo.Upgrades[i].Upgrades.Count;
            }
        }

        void InitUpgradeLevel(UpgradesLevelPanel panel, PlayerUpgradesConfigData.UpgradesStepData data, int currentLevel, int currentScrap, int startIndex)
        {
            panel.Init(data.MinLevel, data.MinLevel > currentLevel);
            for(var i = 0 ; i < data.Upgrades.Count; i++)
            {
                var upgradeID = i + startIndex;
                panel.AddUpgrade(data.Upgrades[i], currentScrap, Progress.Upgrades.GetUpgrade(_collectionID, i).Level, () =>
                {
                    OnUpgradePressed(upgradeID);
                });
            }
        }

        void OnUpgradePressed(int upgradeID)
        {
            _onIconPressed?.Invoke(_collectionID, upgradeID);
        }

        public void ClearAll()
        {
            _upgradeLevelsPool.Instances.ForEach(upgradeLevel=>upgradeLevel.ClearAll());
            _upgradeLevelsPool.ClearAll();
            _defaultUpgradesLevels.ForEach(panel => panel.ClearAll());
        }
    }
}