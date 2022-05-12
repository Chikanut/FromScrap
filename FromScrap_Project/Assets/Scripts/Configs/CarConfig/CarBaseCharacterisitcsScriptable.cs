using StatisticsSystem.Components;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsConfig", menuName = "Configs/Cars/CarCharacteristics", order = 4)]
public class CarBaseCharacterisitcsScriptable : ScriptableObject
{
    [Header("Settings")]
    public int Health;
    [Header("Characteristics")]
    public Characteristics BaseStats;
}
