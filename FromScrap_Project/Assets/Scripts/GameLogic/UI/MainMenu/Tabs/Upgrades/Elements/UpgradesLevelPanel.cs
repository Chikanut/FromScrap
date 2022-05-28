using System;
using FromScrap.Tools;
using Packages.Common.Storage.Config;
using TMPro;
using UnityEngine;

public class UpgradesLevelPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _levelNum;
    [SerializeField] private GameObject _blocker;
    [SerializeField] ObjectPool<UpgradeIconView> _upgradeIconPool;
    
    public void Init(int levelNum, bool isBlocked)
    {
        _levelNum.text = levelNum.ToString();
        _blocker.SetActive(isBlocked);
        _upgradeIconPool.Parent.gameObject.SetActive(!isBlocked);
    }
    
    public void AddUpgrade(PlayerUpgradesConfigData.UpgradeData upgrade, int currentScrap, int currentLevel, Action onPress)
    {
        var canBuy = currentScrap >= upgrade.UpgradesLevels[currentLevel].Cost;
        var upgradeIcon = _upgradeIconPool.GetNextObject();
        upgradeIcon.Reset();
        
        upgradeIcon.Init(upgrade.Icon, "", onPress);
        upgradeIcon.ShowUpgrades(currentLevel, true);
        upgradeIcon.EnableButton(canBuy);
        upgradeIcon.SetState(canBuy
            ? UpgradeIconView.UpgradeIconState.active
            : UpgradeIconView.UpgradeIconState.disabled);
        upgradeIcon.SetMaxLevel(upgrade.UpgradesLevels.Count);
    }
    
    public void ClearAll()
    {
        _upgradeIconPool.ClearAll();
    }
}
