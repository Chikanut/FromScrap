using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Configs/EnemySpawnerConfig", order = 1)]
    public class EnemySpawnerConfigScriptable : ScriptableObject
    {
        [SerializeField] public EnemySpawnerConfigData EnemySpawnerData;
    }
}
