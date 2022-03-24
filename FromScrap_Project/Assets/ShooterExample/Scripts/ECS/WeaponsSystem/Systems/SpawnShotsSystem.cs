using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    [UpdateAfter(typeof(RegularShotSystem))]
    public partial class SpawnShotsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        
        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            
            Entities.ForEach((ref SpawnShotData spawnShotData, in MuzzleData muzzleData, in LocalToWorld localToWorld) =>
            {
                if(spawnShotData.NumberShotsToSpawn == 0){return;}

                if (spawnShotData.NumberShotsToSpawn > 1)
                {
                    var angleIncrement = spawnShotData.ShotSpreadAngle / (spawnShotData.NumberShotsToSpawn - 1);
                    var curAngle = 0 - (spawnShotData.ShotSpreadAngle / 2f);
                    for (var i = 0; i < spawnShotData.NumberShotsToSpawn; i++)
                    {
                        var newShot = SpawnNewShot(ref spawnShotData, muzzleData, localToWorld, ecb);
                        var newRotation = new Rotation {Value = quaternion.AxisAngle(new float3(0f,1f, 0f), math.radians(curAngle))};
                        ecb.SetComponent(newShot, newRotation);
                        curAngle += angleIncrement;
                    }
                }
                else
                {
                    SpawnNewShot(ref spawnShotData, muzzleData, localToWorld, ecb);
                }

                spawnShotData = new SpawnShotData();
            }).Run();
        }

        private static Entity SpawnNewShot(ref SpawnShotData spawnShotData, in MuzzleData muzzleData, in LocalToWorld localToWorld, EntityCommandBuffer ecb)
        {
            var newShot = ecb.Instantiate(spawnShotData.ShotPrefab);
            var newRotation = new Rotation()
            {
                Value = quaternion.LookRotation(localToWorld.Forward, localToWorld.Up)
            };
            var newTranslation = new Translation()
            {
                Value = localToWorld.Value.LocalToWorld(muzzleData.Offset)
            };
            
            ecb.SetComponent(newShot, newRotation);
            ecb.SetComponent(newShot, newTranslation);
            
            return newShot;
        }
    }
}