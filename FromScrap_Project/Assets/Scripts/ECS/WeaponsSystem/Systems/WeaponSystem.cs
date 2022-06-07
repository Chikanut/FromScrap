using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;
using ECS.BlendShapesAnimations.Components;
using ECS.FindTargetSystem;
using Reese.Math;
using StatisticsSystem.Components;
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
            Entities.WithAll<ShotIfRotatedTag, HasTarget>().ForEach(
                (ref IsShotData isShot, in RotateTowardsTarget rotateTowards) =>
                {
                    isShot.Value = rotateTowards.IsRotated;
                }).ScheduleParallel();

            Entities.WithAll<ShotIfRotatedTag>().WithNone<HasTarget>().ForEach(
                (ref IsShotData isShot) => { isShot.Value = false; }).ScheduleParallel();
            
            Entities.WithAll<ShotIfReadyTag, HasTarget>().ForEach(
                (ref IsShotData isShot) => { isShot.Value = true; }).ScheduleParallel();

            Entities.WithAll<ShotIfReadyTag>().WithNone<HasTarget>().ForEach(
                (ref IsShotData isShot) => { isShot.Value = false; }).ScheduleParallel();

            var time = Time.ElapsedTime;
            
            var animationComponentFilter = GetComponentDataFromEntity<BlendShapeAnimationComponent>(true);
            var statisticsFilter = GetComponentDataFromEntity<CharacteristicsComponent>(true);

            var writerProjectileEvent = _eventSystem.CreateEventWriter<SpawnProjectileEvent>();
            
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            var randomIndex = GetSingleton<RandomIndex>().Value;
            
            SetSingleton(new RandomIndex() {Value = randomIndex + 1});

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, ref WeaponData weaponData,
                    in DynamicBuffer<MuzzlesBuffer> muzzlesBuffer,
                    in IsShotData isShot, in LocalToWorld localToWorld) =>
                {
                    var statistics = new Characteristics(1);
                    
                    if(statisticsFilter.HasComponent(entity))
                        statistics = statisticsFilter[entity].Value;
                    
                    //Check weapon states
                    switch (weaponData.CurrentState)
                    {
                        case WeaponState.None:
                            weaponData.NewState = WeaponState.NoTarget;
                            break;
                        case WeaponState.NoTarget:
                            if (isShot.Value)
                                weaponData.NewState = WeaponState.Charge;
                            break;
                        case WeaponState.Charge:
                            if (time - weaponData.PrevStateChangeTime > weaponData.ChargeTime / statistics.ChargeSpeedMultiplier)
                                weaponData.NewState = WeaponState.Shoot;
                            break;
                        case WeaponState.Shoot:
                            if (time - weaponData.PrevStateChangeTime > weaponData.ShootTime)
                                weaponData.NewState = WeaponState.Reload;
                            break;
                        case WeaponState.Reload:
                            if (time - weaponData.PrevStateChangeTime > weaponData.ReloadTime / statistics.ReloadSpeedMultiplier)
                                weaponData.NewState = WeaponState.NoTarget;
                            break;
                        default:
                            weaponData.NewState = WeaponState.NoTarget;
                            break;
                    }

                    //Check is state changed
                    if (weaponData.NewState == weaponData.CurrentState) return;
                    
                    //Update current state
                    weaponData.CurrentState = weaponData.NewState;
                    weaponData.PrevStateChangeTime = time;

                    //Shoot if state is shot
                    if (weaponData.CurrentState == WeaponState.Shoot)
                    {
                        if (muzzlesBuffer.Length == 0)
                            return;

                        switch (weaponData.MuzzleType)
                        {
                            case MuzzleType.Queue:
                            {
                                var muzzleData = muzzlesBuffer[weaponData.CurrentMuzzle];

                                ShotProjectile(muzzleData, writerProjectileEvent, localToWorld,statistics,
                                    randomIndex + weaponData.CurrentMuzzle);

                                weaponData.CurrentMuzzle =
                                    (weaponData.CurrentMuzzle + 1) % muzzlesBuffer.Length;

                                break;
                            }
                            case MuzzleType.All:
                            {
                                for (var i = 0; i < muzzlesBuffer.Length; i++)
                                {
                                    var muzzleData = muzzlesBuffer[i];
                                    ShotProjectile(muzzleData, writerProjectileEvent, localToWorld,statistics,
                                        randomIndex + i);
                                }

                                break;
                            }
                        }
                    }

                    //Show animation of current state
                    MuzzlesAnimation(muzzlesBuffer, weaponData, ecb, entityInQueryIndex,
                        animationComponentFilter);
                }).WithReadOnly(animationComponentFilter).WithReadOnly(statisticsFilter)
                .ScheduleParallel(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
            _eventSystem.AddJobHandleForProducer<SpawnProjectileEvent>(Dependency);
        }

        private static void ShotProjectile(MuzzlesBuffer muzzleData,
            NativeEventStream.ThreadWriter writerProjectileEvent, LocalToWorld localToWorld, Characteristics characteristics, int random)
        {
            var muzzlePos = muzzleData.Offset.ToWorld(localToWorld);

            var shotsCount = muzzleData.ShotsCount + characteristics.AdditionalProjectiles;

            var angleStep = muzzleData.ShotsAngle / (shotsCount);
            var startAngle = -(muzzleData.ShotsAngle / 2f) - (angleStep / 2f);

            for (int j = 0; j < shotsCount; j++)
            {
                var angle = math.radians(startAngle + angleStep * (j + 1));
                var direction = math.mul(quaternion.AxisAngle(muzzleData.ShotsAngleAxis, angle),
                    muzzleData.Direction);
                var dir = math.normalize(direction.ToWorld(localToWorld) - localToWorld.Position);
                var forward = AddSprayToDir(dir, muzzleData.ShootSpray, random + j);

                writerProjectileEvent.Write(new SpawnProjectileEvent()
                {
                    SpawnPos = muzzlePos,
                    SpawnDir = forward,
                    SpawnProjectileName = muzzleData.Projectile,
                    SpeedMultiplier = characteristics.ProjectileSpeedMultiplier,
                    DamageMultiplier = characteristics.DamageMultiplier,
                    SizeMultiplier = characteristics.ProjectileSizeMultiplier,
                    AdditionalDamage = characteristics.AdditionalDamage
                });
            }
        }

        private static Vector3 AddSprayToDir(float3 dir, float spray, int r)
        {
            var random = Random.CreateFromIndex((uint) r);
            var randomDir = random.NextFloat3Direction();

            randomDir = new float3(
                math.clamp(math.sign(randomDir.x) * (math.abs(randomDir.x) - math.abs(dir.x)), -1, 1),
                math.clamp(math.sign(randomDir.y) * (math.abs(randomDir.y) - math.abs(dir.y)), -1, 1),
                math.clamp(math.sign(randomDir.z) * (math.abs(randomDir.z) - math.abs(dir.z)), -1, 1));

            return math.normalize(dir + randomDir * (spray / 90));
        }

        private static void MuzzlesAnimation(DynamicBuffer<MuzzlesBuffer> muzzlesBuffer, WeaponData weaponData,
            EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex,
            ComponentDataFromEntity<BlendShapeAnimationComponent> animationComponentFilter)
        {
           
            
            switch (weaponData.MuzzleType)
            {
                case MuzzleType.Queue:
                {
                    var muzzleData = muzzlesBuffer[weaponData.CurrentMuzzle];
                    
                    MuzzleAnimation(GetAnimationIndex(muzzleData, weaponData.CurrentState),
                        GetAnimationTime(weaponData), muzzleData.MuzzleView, ecb, entityInQueryIndex,
                        animationComponentFilter);

                    weaponData.CurrentMuzzle = (weaponData.CurrentMuzzle + 1) % muzzlesBuffer.Length;

                    break;
                }
                case MuzzleType.All:
                {
                    for (var i = 0; i < muzzlesBuffer.Length; i++)
                    {
                        var muzzleData = muzzlesBuffer[i];
                        MuzzleAnimation(GetAnimationIndex(muzzleData, weaponData.CurrentState),
                            GetAnimationTime(weaponData), muzzleData.MuzzleView, ecb, entityInQueryIndex,
                            animationComponentFilter);
                    }

                    break;
                }
            }
        }

        private static int GetAnimationIndex(MuzzlesBuffer muzzleData, WeaponState state)
        {
            switch (state)
            {
                case WeaponState.Shoot: return muzzleData.ShotAnimationIndex;
                case WeaponState.Reload: return muzzleData.ReloadAnimationIndex;
                case WeaponState.Charge: return muzzleData.ChargeAnimationIndex;
                case WeaponState.NoTarget: return muzzleData.IdleAnimationIndex;
                default: return muzzleData.IdleAnimationIndex;
            }
        }

        private static float GetAnimationTime(WeaponData weaponData)
        {
            switch (weaponData.CurrentState)
            {
                case WeaponState.Shoot: return weaponData.ShootTime;
                case WeaponState.Reload: return weaponData.ReloadTime;
                case WeaponState.Charge: return weaponData.ChargeTime;
                case WeaponState.NoTarget: return 0;
                default: return 0;
            }
        }

        private static void MuzzleAnimation(int animationIndex, float time, Entity viewEntity,
            EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex,
            ComponentDataFromEntity<BlendShapeAnimationComponent> animationComponentFilter)
        {
            if (animationIndex < 0) return;
            if (animationComponentFilter.HasComponent(viewEntity))
            {
                ecb.SetComponent(entityInQueryIndex, viewEntity, new BlendShapeAnimationComponent()
                {
                    AnimationIndex = animationIndex,
                    OverrideTime = time > 0,
                    Time = time
                });
            }
            else
            {
                ecb.AddComponent(entityInQueryIndex, viewEntity, new BlendShapeAnimationComponent()
                {
                    AnimationIndex = animationIndex,
                    OverrideTime = time > 0,
                    Time = time
                });
            }
        }

    }
}