using System.Collections;
using System.Collections.Generic;
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

    }

    protected override void OnMediatorEnable()
    {
        base.OnMediatorEnable();
            
        SignalService.Publish(new OnMainMenuChangeViewAction()
        {
            ViewName = View.TabName
        });

        View.UpdateCollections(Progress.Player.Level, Progress.Player.Scrap,
            _playerProgressionConfigController.GetPlayerProgressionData.Upgrades, OnUpgradePressed);
    }

    void OnUpgradePressed(int collectionID, int upgradeID)
    {
        Debug.Log("Upgrade pressed: " + collectionID + " " + upgradeID);
    }
}
