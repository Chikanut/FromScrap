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
        }
    }
}