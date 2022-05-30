using System.Linq;
using I2.Loc;
using Packages.Common.Storage.Config;
using ShootCommon.Views.Mediation;
using UnityEngine;
using Visartech.Progress;
using Zenject;

public class UpgradesTabMediator : Mediator<UpgradesTab>
{
    private IPlayerProgressionConfigController _playerProgressionConfigController;
    
    [Inject]
    public void Init(IPlayerProgressionConfigController playerProgressionConfigController)
    {
        _playerProgressionConfigController = playerProgressionConfigController;
    }

    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();
        
        View.OnBuyButtonClicked += OnBuyButtonClicked;
        View.CheckIsNewAction += CheckIfNew;
        View.OnTabSelected += OnTabSelected;
    }

    public bool _hasSelectedUpgrade;
    public Vector2Int _upgradeCoordinates;
    
    private void OnTabSelected()
    {
        Progress.Upgrades.PreviousLevel = Progress.Player.Level;
        
        SignalService.Publish(new OnMainMenuChangeViewAction()
        {
            ViewName = View.TabName
        });

        InitView();
        
        _hasSelectedUpgrade = false;

        SignalService.Publish(new OnNewUpdateAction());
    }

    private void OnBuyButtonClicked()
    {
        if (_hasSelectedUpgrade)
        {
            var upgrade =
                _playerProgressionConfigController.GetPlayerProgressionData.Upgrades.GetUpgradeData(
                    _upgradeCoordinates.x, _upgradeCoordinates.y);
            var progress = Progress.Upgrades.GetUpgrade(_upgradeCoordinates.x, _upgradeCoordinates.y);
            if (progress.Level < upgrade.UpgradesLevels.Count)
            {
                var getPrice = upgrade.UpgradesLevels[progress.Level].Cost;
                if (Progress.Player.Scrap >= getPrice)
                {
                    Progress.Player.Scrap -= getPrice;
                    progress.Level++;
                    InitView();
                    OnUpgradePressed(_upgradeCoordinates.x, _upgradeCoordinates.y);
                    
                    SignalService.Publish(new OnPlayerInfoChanged());
                }
            }
        }
    }
    
    void InitView()
    {
        View.UpdateCollections(Progress.Player.Level, Progress.Player.Scrap,
            _playerProgressionConfigController.GetPlayerProgressionData.Upgrades, OnUpgradePressed);
    }

    void OnUpgradePressed(int collectionID, int upgradeID)
    {
        _upgradeCoordinates = new Vector2Int(collectionID, upgradeID);
        _hasSelectedUpgrade = true;
        
        var upgrade = _playerProgressionConfigController.GetPlayerProgressionData.Upgrades.GetUpgradeData(collectionID, upgradeID);
        var progress = Progress.Upgrades.GetUpgrade(collectionID, upgradeID);

        if (progress.Level >= upgrade.UpgradesLevels.Count)
        {
            View.ShowUpgradeInfo(GetUpgradeDescription(upgrade.UpgradesLevels[upgrade.UpgradesLevels.Count-1]));
        }
        else
        {
            var canBuy = upgrade.UpgradesLevels[progress.Level].Cost <= Progress.Player.Scrap;
            View.ShowUpgradeBuyInfo(upgrade.UpgradesLevels[progress.Level].Cost,
                GetUpgradeDescription(upgrade.UpgradesLevels[progress.Level]), canBuy);
        }
    }
    
    string GetUpgradeDescription(PlayerUpgradesConfigData.UpgradeLevelData upgradeLevelData)
    {
        var descriptionText ="";
            
        foreach (var description in upgradeLevelData.Descriptions)
        {
            var descriptionTranslation = LocalizationManager.GetTranslation(description.DescriptionKey);
                
            for(var i = 0 ; i < description.Values.Length ; i ++)
                descriptionTranslation = descriptionTranslation.Replace("{" + i + "}", description.Values[i].ToString());
                
            descriptionText += descriptionTranslation + "\n";
        }

        return descriptionText;
    }

    public bool CheckIfNew()
    {
        if (Progress.Player.Level > Progress.Upgrades.PreviousLevel &&
            _playerProgressionConfigController.GetPlayerProgressionData.Upgrades.UpgradesCollections.Any(collection =>
                collection.Upgrades.Any(upgradeLevel =>
                    upgradeLevel.MinLevel > Progress.Upgrades.PreviousLevel &&
                    upgradeLevel.MinLevel <= Progress.Player.Level)))
        {
            return true;
        }
        
        return false;
    }

}
