using StatisticsSystem.Components;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class CharacteristicsPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _maxHealthValue;
    [SerializeField] private TextMeshProUGUI _damageResistValue;
    [SerializeField] private TextMeshProUGUI _healthRestoreValue;
    [SerializeField] private TextMeshProUGUI _speedValue;
    
    [SerializeField] private TextMeshProUGUI _areaSizeValue;
    [SerializeField] private TextMeshProUGUI _damageValue;
    [SerializeField] private TextMeshProUGUI _reloadSpeedValue;
    [SerializeField] private TextMeshProUGUI _effectDuration;
    [SerializeField] private TextMeshProUGUI _projectileSpeedValue;
    [SerializeField] private TextMeshProUGUI _projectileSizeValue;
    [SerializeField] private TextMeshProUGUI _additionalProjectileValue;
    
    [SerializeField] private TextMeshProUGUI _luckValue;
    [SerializeField] private TextMeshProUGUI _growthValue;
    [SerializeField] private TextMeshProUGUI _greedValue;

    public void UpdateCharacteristics(Characteristics baseStats, Characteristics currentStats)
    {
        UpdatePercentageValues(baseStats, currentStats);
        UpdatePointsValues(baseStats, currentStats);
    }

    void UpdatePointsValues(Characteristics baseStats, Characteristics currentStats)
    {
        _additionalProjectileValue.text = (currentStats.AdditionalProjectiles - baseStats.AdditionalProjectiles).ToString();
        _maxHealthValue.text = ((int) (currentStats.MaxHealth * currentStats.HealthMultiplier + currentStats.AdditionalHealth)).ToString();
    }

    void UpdatePercentageValues(Characteristics baseStats, Characteristics currentStats)
    {
        UpdatePercentage(_damageResistValue, baseStats.DamageResistMultiplier, currentStats.DamageResistMultiplier);
        UpdatePercentage(_healthRestoreValue, baseStats.HealthRestoreMultiplier, currentStats.HealthRestoreMultiplier);
        UpdatePercentage(_speedValue, baseStats.MovementSpeedMultiplier + baseStats.TorqueMultiplier,
            currentStats.MovementSpeedMultiplier + currentStats.TorqueMultiplier);
        
        UpdatePercentage(_areaSizeValue, baseStats.AreaMultiplier, currentStats.AreaMultiplier);
        UpdatePercentage(_damageValue, baseStats.DamageMultiplier, currentStats.DamageMultiplier);
        UpdatePercentage(_effectDuration, baseStats.EffectDurationMultiplier, currentStats.EffectDurationMultiplier);
        UpdatePercentage(_reloadSpeedValue, baseStats.ReloadSpeedMultiplier, currentStats.ReloadSpeedMultiplier);
        UpdatePercentage(_projectileSpeedValue, baseStats.ProjectileSpeedMultiplier, currentStats.ProjectileSpeedMultiplier);
        UpdatePercentage(_projectileSizeValue, baseStats.ProjectileSizeMultiplier, currentStats.ProjectileSizeMultiplier);
        
        UpdatePercentage(_luckValue, baseStats.LuckMultiplier, currentStats.LuckMultiplier);
        UpdatePercentage(_growthValue, baseStats.GrowthMultiplier, currentStats.GrowthMultiplier);
        UpdatePercentage(_greedValue, baseStats.GreedMultiplier, currentStats.GreedMultiplier);
    }

    void UpdatePercentage(TextMeshProUGUI text, float baseValue, float currentValue)
    {
        var percent = baseValue / 100f;
        var currentPercent = (int)(currentValue / percent);
        var currentPercentageDifference = currentPercent - 100;

        if (math.abs(currentPercentageDifference) < 1)
        {
            text.text = "-";
        }
        else
        {
            text.text = (math.sign(currentPercentageDifference) > 0 ? "+" : "-") + $"{currentPercentageDifference}%";
        }
    }
}
