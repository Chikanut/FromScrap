using BovineLabs.Event.Systems;
using DamageSystem.Components;
using ECS.SignalSystems.Systems;
using StatisticsSystem.Components;
using Unity.Entities;
using Unity.Transforms;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(DeathCleanupSystem))]
    public partial class ResolveDamageSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        private EventSystem _eventSystem;
        
        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _eventSystem = this.World.GetOrCreateSystem<EventSystem>();
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var onDamageTextEvent = _eventSystem.CreateEventWriter<DamagePointsEvent>();
            var onEnemyDamageEvent = _eventSystem.CreateEventWriter<EnemyDamageSignal>();
            var onEnemyKillEvent = _eventSystem.CreateEventWriter<EnemyKillSignal>();
            
            var ecb = _ecbSystem.CreateCommandBuffer();
            var damageBlockers = GetComponentDataFromEntity<DamageBlockTimer>(true);
            var characteristicsFilter = GetComponentDataFromEntity<CharacteristicsComponent>(true);
            
            
            Dependency = Entities.WithName("ProcessDamagePoints").WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref Health health, in LocalToWorld localToWorld) =>
            {
                foreach (var damage in damages)
                {
                    var dmgValue = damage.Value;

                    if (characteristicsFilter.HasComponent(entity))
                    {
                        dmgValue = (int)(dmgValue / characteristicsFilter[entity].Value.DamageResistMultiplier);
                    }

                    if (damage.isPlayer)
                    {
                        onEnemyDamageEvent.Write(new EnemyDamageSignal(){Damage = dmgValue});
                    }

                    health.Value -= dmgValue;

                    if (health.OnDamageBlockTime > 0)
                    {
                        if(damageBlockers.HasComponent(entity))
                            ecb.SetComponent(entity, new DamageBlockTimer(){Value = health.OnDamageBlockTime});
                        else
                            ecb.AddComponent(entity, new DamageBlockTimer(){Value = health.OnDamageBlockTime});
                    }
                    
                    if(health.ShowHitsNumbers)
                        onDamageTextEvent.Write(new DamagePointsEvent(){Damage = damage.Value, Position = localToWorld.Position});

                    if (health.Value > 0) continue;

                    health.Value = 0;
                    ecb.AddComponent<Dead>(entity);

                    if(damage.isPlayer)
                        onEnemyKillEvent.Write(new EnemyKillSignal());
                    
                    break;
                }

                damages.Clear();
            }).WithReadOnly(characteristicsFilter).WithReadOnly(damageBlockers).Schedule(Dependency);
            
            _eventSystem.AddJobHandleForProducer<DamagePointsEvent>(Dependency);
            _eventSystem.AddJobHandleForProducer<EnemyDamageSignal>(Dependency);
            _eventSystem.AddJobHandleForProducer<EnemyKillSignal>(Dependency);

            var deltaTime = Time.DeltaTime;
            
            Entities.WithName("DamageBlockerUpdate").WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref DamageBlockTimer blockTimer) =>
            {
                blockTimer.Value -= deltaTime;
            
                if (blockTimer.Value <= 0)
                    ecb.RemoveComponent<DamageBlockTimer>(entity);
            
                damages.Clear();
            }).Schedule();
        }
    }

    [UpdateBefore(typeof(ResolveDamageSystem))]
    public partial class RecalculateMaxHealthSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithName("RecalculateMaxHealthSystem").ForEach((Entity entity, ref Health health, in CharacteristicsComponent characteristic) =>
            {
                var maxHealth = (int)(characteristic.Value.MaxHealth * characteristic.Value.HealthMultiplier + characteristic.Value.AdditionalHealth);
                
                if (health.CurrentMaxValue != maxHealth)
                    health.SetMaxHealth(maxHealth);
            }).Schedule();
        }
    }
}