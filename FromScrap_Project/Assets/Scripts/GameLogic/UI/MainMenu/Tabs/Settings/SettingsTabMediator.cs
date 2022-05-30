using ShootCommon.Views.Mediation;
using UnityEngine;

public class SettingsTabMediator : Mediator<SettingsTab>
{
    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();
        
        View.OnExit += OnExit;
        View.CheckIsNewAction += CheckIfNew;
        View.OnTabSelected += OnTabSelected;
    }

    private void OnExit()
    {
        Application.Quit();
    }

    private void OnTabSelected()
    {
        SignalService.Publish(new OnMainMenuChangeViewAction()
        {
            ViewName = View.TabName
        });
    }

    private bool CheckIfNew()
    {
        return false;
    }
}
