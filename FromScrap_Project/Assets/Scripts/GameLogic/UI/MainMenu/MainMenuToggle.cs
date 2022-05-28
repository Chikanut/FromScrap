using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject _new;
    [SerializeField] private Toggle _toggle;

    public Action<bool> OnActive;
    
    public void Init()
    {
        _toggle.onValueChanged.AddListener(OnValueChanged);
    }

    public void Enable()
    {
        _toggle.isOn = true;
    }

    private void OnValueChanged(bool arg0)
    {
        OnActive?.Invoke(arg0);
    }

    public void SetNew(bool enable)
    {
        _new.SetActive(enable);
    }
}
