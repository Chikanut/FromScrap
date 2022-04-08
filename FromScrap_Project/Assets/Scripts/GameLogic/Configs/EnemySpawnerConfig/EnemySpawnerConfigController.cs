using System;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    public class EnemySpawnerConfigController : IEnemySpawnerConfigController
    {
        private EnemySpawnerConfigData _model;
        
        public EnemySpawnerConfigData GetEnemySpawnerData => _model;
        
        public virtual void SetInfo(EnemySpawnerConfigScriptable data)
        {
            _model = data.EnemySpawnerData;
            
            //sort enemies by spawn chance (lower chance goes first)
            foreach (var range in _model.SpawnRanges)
            {
                range.EnemySpawnInfos.Sort((e1, e2) =>
                    e1.SpawnChance > e2.SpawnChance ? 1 : Math.Abs(e1.SpawnChance - e2.SpawnChance) < 0.005f ? 0 : -1);
            }
        }
    }
}