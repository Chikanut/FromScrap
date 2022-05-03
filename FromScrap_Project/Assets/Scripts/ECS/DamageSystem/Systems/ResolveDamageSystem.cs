using BovineLabs.Event.Systems;
using DamageSystem.Components;
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
            var ecb = _ecbSystem.CreateCommandBuffer();
            var damageBlockers = GetComponentDataFromEntity<DamageBlockTimer>(true);

            Dependency = Entities.WithName("ProcessDamagePoints").WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref Health health, in LocalToWorld localToWorld) =>
            {
                foreach (var damage in damages)
                {
                    health.Value -= damage.Value;

                    if (health.OnDamageBlockTime > 0)
                    {
                        if(damageBlockers.HasComponent(entity))
                            ecb.SetComponent(entity, new DamageBlockTimer(){Value = health.OnDamageBlockTime});
                        else
                            ecb.AddComponent<DamageBlockTimer>(entity, new DamageBlockTimer(){Value = health.OnDamageBlockTime});
                    }
                    
                    if(health.ShowHitsNumbers)
                        onDamageTextEvent.Write(new DamagePointsEvent(){Damage = damage.Value, Position = localToWorld.Position});

                    if (health.Value > 0) continue;

                    health.Value = 0;
                    ecb.AddComponent<Dead>(entity);

                    break;
                }

                damages.Clear();
            }).WithReadOnly(damageBlockers).Schedule(Dependency);
            
            _eventSystem.AddJobHandleForProducer<DamagePointsEvent>(Dependency);

            float deltaTime = Time.DeltaTime;
            
            Entities.WithName("DamageBlockerUpdate").WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref DamageBlockTimer blockTimer) =>
            {
                blockTimer.Value -= deltaTime;
            
                if (blockTimer.Value <= 0)
                    ecb.RemoveComponent<DamageBlockTimer>(entity);
            
                damages.Clear();
            }).Schedule();
        }
    }
}