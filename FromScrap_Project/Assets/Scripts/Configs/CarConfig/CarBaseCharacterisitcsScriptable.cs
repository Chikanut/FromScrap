using System.Collections;
using System.Collections.Generic;
using StatisticsSystem.Components;
using UnityEngine;

[CreateAssetMenu(fileName = "CarsConfig", menuName = "Configs/Cars/CarCharacteristics", order = 4)]
public class CarBaseCharacterisitcsScriptable : ScriptableObject
{
    public int Health;
    public Statistics BaseStats;
}
