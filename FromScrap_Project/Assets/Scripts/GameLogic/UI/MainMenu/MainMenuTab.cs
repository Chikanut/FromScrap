using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using UnityEngine;

public struct OnMainMenuChangeViewAction : ISignal
{
    public string ViewName;
}

public struct OnNewUpdateAction : ISignal { }

public class MainMenuTab : View
{
    [Header("Base")]
    public string TabName = "default";
    public virtual bool HasNew => false;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
