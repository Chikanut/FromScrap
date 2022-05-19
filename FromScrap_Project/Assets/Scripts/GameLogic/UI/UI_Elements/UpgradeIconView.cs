using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIconView : MonoBehaviour
{
    public enum UpgradeIconState
    {
        active,
        selected,
        disabled,
        hide,
    }
    
    [Header("Components")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _frame;
    [SerializeField] private List<Image> _upgradeLevels = new List<Image>();
    [SerializeField] private GameObject _maxLabel;
    [SerializeField] private GameObject _plusSign;
    [SerializeField] private GameObject _upgradesHolder;
    [SerializeField] private GameObject _shine;
    [SerializeField] private Button _button;

    [Header("Settings")]
    [SerializeField] private UpgradeIconState _defaultState = UpgradeIconState.active;
    [Space]
    [SerializeField] Color _upgradeLevelDefaultColor = Color.white;
    [SerializeField] Color _upgradeLevelActiveColor = Color.yellow;
    [Space]
    [SerializeField] Color _frameDefaultColor = Color.white;
    [SerializeField] Color _frameSelectedtColor = Color.white;
    [SerializeField] Color _frameDisableColor = Color.gray;
    [SerializeField] Color _frameHideColor = new Color(0.1f, 0.1f, 0.1f, 1);

    private Action _onPressed;

    void Start()
    {
        _button.onClick.AddListener(OnPressed);
    }

    void OnPressed()
    {
        _onPressed?.Invoke();
    }

    public void Init(Sprite icon, Action onPress = null)
    {
        _icon.sprite = icon;
        _icon.color = Color.white;
        _onPressed = onPress;
    }
    
    public void EnableButton(bool enable)
    {
        _button.interactable = enable;
    }

    public void SetState(UpgradeIconState state)
    {
        _shine.SetActive(false);
        
        switch (state)
        {
            case UpgradeIconState.active :
                _frame.color = _frameDefaultColor;
                break;
            case UpgradeIconState.selected:
                _shine.SetActive(true);
                _frame.color = _frameSelectedtColor;
                break;
            case UpgradeIconState.hide:
                _frame.color = _frameHideColor;
                break;
            case UpgradeIconState.disabled:
                _frame.color = _frameDisableColor;
                break;
        }
    }

    public void ShowUpgrades(int currentLevel, bool showMax)
    {
        _upgradesHolder.SetActive(true);
        
        _maxLabel.SetActive(currentLevel >= _upgradeLevels.Count-1 && showMax);

        for (int i = 0; i <  _upgradeLevels.Count ; i++)
        {
            _upgradeLevels[i].color = i < currentLevel ? _upgradeLevelActiveColor : _upgradeLevelDefaultColor;
        }
    }
    
    public void HideUpgrades()
    {
        _upgradesHolder.SetActive(false);
        _maxLabel.SetActive(false);
    }
    
    public void EnablePlusSign(bool enable)
    {
        _plusSign.SetActive(enable);
    }

    public void Reset()
    {
        EnableButton(false);
        EnablePlusSign(false);
        HideUpgrades();
        SetState(_defaultState);
        _icon.color = Color.clear;
    }
}
