using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTab : MainMenuTab
{
    [SerializeField] private Button _exitButton;

    public Action OnExit;

    public void Awake()
    {
        _exitButton.onClick.AddListener(OnExitClick);
    }

    private void OnExitClick()
    {
        OnExit?.Invoke();
    }
}
