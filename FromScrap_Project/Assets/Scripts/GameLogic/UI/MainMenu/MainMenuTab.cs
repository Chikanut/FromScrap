using System;
using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using UnityEngine;

public struct OnMainMenuChangeViewAction : ISignal
{
    public string ViewName;
}

public struct OnNewUpdateAction : ISignal { }

public struct OnPlayerInfoChanged : ISignal { }

public class MainMenuTab : View
{
    [Header("Base")]
    public string TabName = "default";

    public GameObject TabObject;
    public bool HasNew => CheckIsNewAction?.Invoke() ?? false;

    public virtual void Show()
    {
        TabObject.SetActive(true);
        OnTabSelected?.Invoke();
    }

    public virtual void Hide()
    {
        TabObject.SetActive(false);
    }
    
    public Action OnTabSelected;
    public Func<bool> CheckIsNewAction;
}
