using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesConfig", menuName = "Configs/Cars/LevelsConfig", order = 3)]
public class LevelsConfigScriptable : ScriptableObject
{
    public List<int> LevelsExperience = new List<int>();
}
