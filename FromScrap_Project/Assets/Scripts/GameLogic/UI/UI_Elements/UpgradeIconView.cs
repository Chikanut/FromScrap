using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using Unity.Mathematics;
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
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _frame;
    [SerializeField] private List<Image> _upgradeLevels = new List<Image>();
    [SerializeField] private GameObject _maxLabel;
    [SerializeField] private GameObject _plusSign;
    [SerializeField] private GameObject _upgradesHolder;
    [SerializeField] private GameObject _shine;
    [SerializeField] private Button _button;
    [SerializeField] private Toggle _toggle;

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
        if(_button != null)
            _button.onClick.AddListener(OnPressed);
        
        if(_toggle != null)
            _toggle.onValueChanged.AddListener((b) =>
            {
                if(b)
                    OnPressed();
            });
    }

    void OnPressed()
    {
        _onPressed?.Invoke();
    }

    public void Init(Sprite icon, string nameKey = "", Action onPress = null)
    {
        _icon.sprite = icon;
        _icon.color = Color.white;
        _onPressed = onPress;

        if (nameKey != "")
        {
            _name.gameObject.SetActive(true);
            _name.text = LocalizationManager.GetTranslation(nameKey);
        }
        else
        {
            _name.gameObject.SetActive(false);
        }
    }
    
    public void EnableButton(bool enable)
    {
        if(_button != null)
            _button.interactable = enable;
        if (_toggle != null)
            _toggle.interactable = enable;
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

    public void ShowUpgrades(int currentLevel, int maxLevel, bool showMax)
    {
        _upgradesHolder.SetActive(true);
        
        SetMaxLevel(maxLevel);
        _maxLabel.SetActive(currentLevel >= _upgradeLevels.Count-1 && showMax);

        for (int i = 0; i <  _upgradeLevels.Count ; i++)
        {
            _upgradeLevels[i].color = i < currentLevel ? _upgradeLevelActiveColor : _upgradeLevelDefaultColor;
        }
    }

    public void SetMaxLevel(int maxLevel)
    {
        for (var i = math.min(math.max(maxLevel,1), _upgradeLevels.Count) - 1 ; i < _upgradeLevels.Count; i++)
        {
            _upgradeLevels[i].gameObject.SetActive(false);
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

        if (_toggle != null)
            _toggle.isOn = false;
        
        foreach (var level in _upgradeLevels)
            level.gameObject.SetActive(true);

        _icon.color = Color.clear;
        _name.gameObject.SetActive(false);
    }
}
