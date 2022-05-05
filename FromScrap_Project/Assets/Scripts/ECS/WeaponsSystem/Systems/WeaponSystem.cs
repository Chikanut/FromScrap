using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;
using ECS.BlendShapesAnimations.Components;
using Reese.Math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WeaponsSystem.Base.Components;
using Random = Unity.Mathematics.Random;

namespace WeaponsSystem.Base.Systems
{
    public partial class WeaponSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        
        private EventSystem _eventSystem;
        protected override void OnCreate()
        {
            _eventSystem = World.GetOrCreateSystem<EventSystem>();
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
          
            
            Entities.WithAll<ShotIfRotatedTag>().ForEach(
                (ref IsShotData isShot, in RotateTowardsTarget rotateTowards) =>
                {
                    isShot.Value = rotateTowards.IsRotated;
                }).ScheduleParallel();

            var time = Time.ElapsedTime;
            var animationComponentFilter = GetComponentDataFromEntity<BlendShapeAnimationComponent>(true);
            var writerProjectileEvent = _eventSystem.CreateEventWriter<SpawnProjectileEvent>();
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var randomIndex = GetSingleton<RandomIndex>().Value;
            SetSingleton(new RandomIndex() {Value = randomIndex + 1});

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, ref WeaponData weaponData, in DynamicBuffer<MuzzlesBuffer> muzzlesBuffer,
                    in IsShotData isShot, in LocalToWorld localToWorld) =>
                {
                    if (!isShot.Value || !(time - weaponData.PrevShotTime > weaponData.ShotFrequency)) return;

                    if (muzzlesBuffer.Length == 0)
                        return;

                    switch (weaponData.MuzzleType)
                    {
                        case MuzzleType.Queue:
                        {
                            var muzzleData = muzzlesBuffer[weaponData.CurrentMuzzle];

                            ShotProjectile(muzzleData, writerProjectileEvent, localToWorld, randomIndex + weaponData.CurrentMuzzle);
                            
                            if(muzzleData.ShotAnimationIndex >= 0 )
                            {
                                if (animationComponentFilter.HasComponent(weaponData.WeaponView))
                                {
                                    ecb.SetComponent(entityInQueryIndex, weaponData.WeaponView, new BlendShapeAnimationComponent()
                                    {
                                        AnimationIndex = muzzleData.ShotAnimationIndex,
                                        OverrideTime = true,
                                        Time = weaponData.ShotFrequency
                                    });
                                }
                                else
                                {
                                    ecb.AddComponent(entityInQueryIndex, weaponData.WeaponView, new BlendShapeAnimationComponent()
                                    {
                                        AnimationIndex = muzzleData.ShotAnimationIndex,
                                        OverrideTime = true,
                                        Time = weaponData.ShotFrequency
                                    });
                                }
                            }

                            weaponData.CurrentMuzzle = (weaponData.CurrentMuzzle + 1) % muzzlesBuffer.Length;
                            break;
                        }
                        case MuzzleType.All:
                        {
                            for (var i = 0; i < muzzlesBuffer.Length; i++)
                            {
                                var muzzleData = muzzlesBuffer[i];
                                ShotProjectile(muzzleData, writerProjectileEvent, localToWorld, randomIndex + i);

                                if(muzzleData.ShotAnimationIndex < 0) continue;
                                if (animationComponentFilter.HasComponent(weaponData.WeaponView))
                                {
                                    ecb.SetComponent(entityInQueryIndex, weaponData.WeaponView, new BlendShapeAnimationComponent()
                                    {
                                        AnimationIndex = muzzleData.ShotAnimationIndex,
                                        OverrideTime = true,
                                        Time = weaponData.ShotFrequency
                                    });
                                }else
                                {
                                    ecb.AddComponent(entityInQueryIndex, weaponData.WeaponView, new BlendShapeAnimationComponent()
                                    {
                                        AnimationIndex = muzzleData.ShotAnimationIndex,
                                        OverrideTime = true,
                                        Time = weaponData.ShotFrequency
                                    });
                                }
                            }

                            break;
                        }
                    }

                    weaponData.PrevShotTime = time;
                }).WithReadOnly(animationComponentFilter)
                .ScheduleParallel(Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
            _eventSystem.AddJobHandleForProducer<SpawnProjectileEvent>(Dependency);
        }
        
        private static void ShotProjectile(MuzzlesBuffer muzzleData, NativeEventStream.ThreadWriter writerProjectileEvent, LocalToWorld localToWorld, int random)
        {
            var muzzlePos = muzzleData.Offset.ToWorld(localToWorld);
            var dir = math.normalize(muzzleData.Direction.ToWorld(localToWorld) - localToWorld.Position);
            var forward = AddSprayToDir(dir, muzzleData.ShootSpray, random);

            writerProjectileEvent.Write(new SpawnProjectileEvent()
            {
                SpawnPos = muzzlePos,
                SpawnDir = forward,
                SpawnProjectileName = muzzleData.Projectile
            });
        }

        private static Vector3 AddSprayToDir(float3 dir, float spray, int r)
        {
            var random = Random.CreateFromIndex((uint)r);
            var randomDir = random.NextFloat3Direction();

            randomDir = new float3(math.clamp(math.sign(randomDir.x) * (math.abs(randomDir.x) - math.abs(dir.x)), -1, 1),
                math.clamp(math.sign(randomDir.y) * (math.abs(randomDir.y) - math.abs(dir.y)), -1, 1),
                math.clamp(math.sign(randomDir.z) * (math.abs(randomDir.z) - math.abs(dir.z)), -1, 1));
            
            return math.normalize(dir + randomDir * (spray / 90));
        }
    }
}