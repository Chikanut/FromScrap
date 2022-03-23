using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Components
{
    public partial class RegularShotSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = Time.ElapsedTime;
            
            Entities.ForEach((ref SpawnShotData spawnShotData, in ShotPrefab shotPrefab, in RotateTowardsTarget rotating) =>
                {
                    if (rotating.IsRotated && time - spawnShotData.PrevShotTime > spawnShotData.ShotFrequency)
                    {
                        spawnShotData.ShotPrefab = shotPrefab.Value;
                        spawnShotData.ShotVelocity = GetComponent<ShotData>(shotPrefab.Value).Velocity;
                        spawnShotData.NumberShotsToSpawn = 1;
                        spawnShotData.ShotSpreadAngle = 0f;
                    }
                }).Run();
        }
    }
}