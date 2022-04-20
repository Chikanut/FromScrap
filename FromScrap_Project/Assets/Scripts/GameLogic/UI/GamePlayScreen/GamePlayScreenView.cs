using System;
using MenuNavigation;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayScreenView : MenuScreen
{
    [SerializeField] private Slider _experienceSlider;

    private Action<float> _setExperience;

 

    public void SetExperience(float value)
    {
        value = Mathf.Clamp01(value);
        
        _experienceSlider.value = value;
    }
}
