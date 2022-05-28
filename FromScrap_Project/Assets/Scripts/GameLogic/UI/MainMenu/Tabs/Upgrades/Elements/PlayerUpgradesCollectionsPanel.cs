using System;
using FromScrap.Tools;
using Packages.Common.Storage.Config;
using UI.Screens.MainMenu.Tabs.Upgrades.Elements;
using UnityEngine;

public class PlayerUpgradesCollectionsPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] ObjectPool<UpgradesCollectionPanel> _upgradesCollectionPanelPool;
    
    private Action<int, int> _onIconPressed;
    
    public void Init(int currentLevel, int currentScrap, PlayerUpgradesConfigData playerUpgradesData, Action<int,int> onIconPressed)
    {
        _onIconPressed = onIconPressed;
        
        ClearAll();
        
        var upgradesCollections = playerUpgradesData.UpgradesCollections;

        for (int i = 0; i < upgradesCollections.Count; i++)
        {
            var panel = _upgradesCollectionPanelPool.GetNextObject();
            panel.Init(currentLevel, currentScrap, upgradesCollections[i], i, OnUpgradeSelected);
        }
    }

    void OnUpgradeSelected(int collectionID, int upgradeID)
    {
        _onIconPressed?.Invoke(collectionID, upgradeID);
    }

    void ClearAll()
    {
        _upgradesCollectionPanelPool.Instances.ForEach(collection=>collection.ClearAll());
        _upgradesCollectionPanelPool.ClearAll();
    }
}
