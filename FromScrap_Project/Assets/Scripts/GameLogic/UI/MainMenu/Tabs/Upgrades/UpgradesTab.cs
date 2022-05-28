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
    [SerializeField] private GameObject _costObject;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] ToggleGroup _toggleGroup;

    public Action OnBuyButtonClicked;
    
    private void Awake()
    {
        _buyUpgrade.onClick.AddListener(BuyUpgrade);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _toggleGroup.allowSwitchOff = true;
        HideInfoPanel();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        _toggleGroup.allowSwitchOff = true;
    }

    public void UpdateCollections(int currentLevel, int currentScrap, PlayerUpgradesConfigData playerUpgradesData, Action<int,int> onIconPressed)
    {
        _upgradesCollectionsPanel.Init(currentLevel, currentScrap, playerUpgradesData, onIconPressed);
    }
    
    public void ShowUpgradeBuyInfo(int upgradeCost, string description)
    {
        _upgradeInfoPanel.SetActive(true);
        _descriptionText.text = description;
        _costText.text = upgradeCost.ToString();
        _costObject.SetActive(true);
        _buyUpgrade.gameObject.SetActive(true);

        _toggleGroup.allowSwitchOff = false;
    }

    public void ShowUpgradeInfo(string description)
    {
        _upgradeInfoPanel.SetActive(true);
        _descriptionText.text = description;
        _costObject.SetActive(false);
        _buyUpgrade.gameObject.SetActive(false);
        
        _toggleGroup.allowSwitchOff = false;
    }

    public void HideInfoPanel()
    {
        _upgradeInfoPanel.SetActive(false);
    }

    void BuyUpgrade()
    {
        OnBuyButtonClicked?.Invoke();
    }

}
