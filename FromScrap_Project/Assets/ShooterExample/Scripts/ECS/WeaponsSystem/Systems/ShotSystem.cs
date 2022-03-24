using Unity.Entities;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    public partial class RegularShotSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = Time.ElapsedTime;

            Entities.WithAll<ShotIfRotatedTag>().ForEach(
                (ref IsShotData isShot, in RotateTowardsTarget rotateTowards) =>
                {
                    isShot.Value = rotateTowards.IsRotated;
                }).ScheduleParallel();
            
            Entities.ForEach((ref SpawnShotData spawnShotData, ref MuzzleData muzzleData, in ShotPrefab shotPrefab, in IsShotData isShot) =>
            {
                if (!isShot.Value || !(time - muzzleData.PrevShotTime > muzzleData.ShotFrequency)) return;
                
                spawnShotData.ShotPrefab = shotPrefab.Value;
                spawnShotData.ShotVelocity = GetComponent<ShotData>(shotPrefab.Value).Velocity;
                spawnShotData.NumberShotsToSpawn = 1;
                spawnShotData.ShotSpreadAngle = 0f;

                muzzleData.PrevShotTime = time;
            }).Run();
        }
    }
}