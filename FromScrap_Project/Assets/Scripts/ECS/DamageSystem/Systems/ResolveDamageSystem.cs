using DamageSystem.Components;
using Unity.Entities;

namespace DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateBefore(typeof(DeathCleanupSystem))]
    public partial class ResolveDamageSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();

            Entities.WithoutBurst().WithNone<Dead, DamageBlockTimer>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref Health health) =>
            {
                foreach (var damage in damages)
                {
                    health.Value -= damage.Value;

                    if (health.Value > 0) continue;

                    health.Value = 0;
                    ecb.AddComponent<Dead>(entity);

                    break;
                }

                damages.Clear();
            }).Run();

            float deltaTime = Time.DeltaTime;
            
            Entities.WithoutBurst().WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref DamageBlockTimer blockTimer) =>
            {
                blockTimer.Value -= deltaTime;

                if (blockTimer.Value <= 0)
                    ecb.RemoveComponent<DamageBlockTimer>(entity);

                damages.Clear();
            }).Run();
        }
    }
}