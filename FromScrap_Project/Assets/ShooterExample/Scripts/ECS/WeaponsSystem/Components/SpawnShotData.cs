using Unity.Entities;
using UnityEngine;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct SpawnShotData : IComponentData
    {
        public Entity ShotPrefab;
        public float ShotVelocity;
        public int NumberShotsToSpawn;
        public float ShotSpreadAngle;
        public float ShotFrequency;
        [HideInInspector] public double PrevShotTime;
    }
}
