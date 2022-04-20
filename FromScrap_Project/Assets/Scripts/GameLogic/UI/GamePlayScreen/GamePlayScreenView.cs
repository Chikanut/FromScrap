using System;
using DG.Tweening;
using MenuNavigation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayScreenView : MenuScreen
{
    [Header("Components")]
    [SerializeField] private Slider _experienceSlider;
    [SerializeField] private TextMeshProUGUI _levelLabel;

    [Header("Settings")] 
    [SerializeField] private float _experienceFillSpeed;
    [SerializeField] private Ease _experienceFillEasing;

    private Action<float> _setExperience;

    private Sequence _experienceFillSequence;
    
    public void SetExperience(float value)
    {
        value = Mathf.Clamp01(value);
        
        _experienceFillSequence?.Kill();
        _experienceFillSequence = DOTween.Sequence();

        if (value > _experienceSlider.value)
        {
            var fillTime = (value - _experienceSlider.value) / _experienceFillSpeed;
            _experienceFillSequence.Insert(0,_experienceSlider.DOValue(value, fillTime).SetEase(_experienceFillEasing));
            _experienceFillSequence.Play();
        }
        else
        {
            _experienceSlider.value = value;
        }
    }

    public void SetCurrentLevel(int level)
    {
        _levelLabel.text = $"LvL {level + 1}";
    }
}
