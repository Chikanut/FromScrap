using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config.EnemySpawner
{
    [Serializable]
    public class EnemySpawnerConfigData 
    {
        [Serializable]
        public class SpawnRange
        {
            public float RangeSpawnDuration;
            public float SpawnsPerSecond;
            public AnimationCurve SpawnIntensityCurve;

            [Serializable]
            public class EnemySpawnInfo
            {
                public GameObject EnemyPrefab;
                [Range(0,100)]
                public float SpawnChance;
            }

            public List<EnemySpawnInfo> EnemySpawnInfos = new List<EnemySpawnInfo>();
        }

        public float SpawnOffset = 4;
        public List<SpawnRange> SpawnRanges = new List<SpawnRange>();
    }
}
