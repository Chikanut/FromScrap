using System.Collections;
using System.Collections.Generic;
using ShootCommon.Views.Mediation;
using UniRx;
using UnityEngine;

public class MainMenuBackTransitionsMediator : Mediator<MainMenuBackTransitionsView>
{
    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();

        SignalService.Receive<OnMainMenuChangeViewAction>().Subscribe(OnMainMenuStateChanged).AddTo(DisposeOnDestroy);
    }

    private void OnMainMenuStateChanged(OnMainMenuChangeViewAction obj)
    {
        View.OnStateChanged(obj.ViewName);
    }
}
