using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config
{
    [Serializable]
    public class EnemySpawnerConfigData 
    {
        [Serializable]
        public class SpawnRange
        {
            public float RangeSpawnDuration;
            public float SpawnsPerSecond;

            [Serializable]
            public class EnemySpawnInfo
            {
                public GameObject EnemyPrefab;
                public float SpawnChance;
            }

            public List<EnemySpawnInfo> EnemySpawnInfos = new List<EnemySpawnInfo>();
        }

        public List<SpawnRange> SpawnRanges = new List<SpawnRange>();
    }
}
