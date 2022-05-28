using System;
using Packages.Common.Storage.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesTab : MainMenuTab
{
    [Header("Upgrades collections")]
    [SerializeField] private PlayerUpgradesCollectionsPanel _upgradesCollectionsPanel;
    [Header("Info panel")]
    [SerializeField] private GameObject _upgradeInfoPanel;
    [SerializeField] private Button _buyUpgrade;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;
    
    private void Awake()
    {
        _buyUpgrade.onClick.AddListener(BuyUpgrade);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _upgradeInfoPanel.SetActive(false);
    }

    public void UpdateCollections(int currentLevel, int currentScrap, PlayerUpgradesConfigData playerUpgradesData, Action<int,int> onIconPressed)
    {
        _upgradesCollectionsPanel.Init(currentLevel, currentScrap, playerUpgradesData, onIconPressed);
    }
    
    public void ShowUpgradeInfo(int upgradeCost, string description)
    {
        _upgradeInfoPanel.SetActive(true);
        _descriptionText.text = description;
        _costText.text = upgradeCost.ToString();
    }

    void BuyUpgrade()
    {
        Debug.LogError("Buy upgrade");
    }

}
