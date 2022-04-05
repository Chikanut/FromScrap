using DamageSystem.Components;
using Unity.Entities;

namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(DeathCleanupSystem))]
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

            Entities.WithoutBurst().WithNone<Dead>().ForEach((Entity entity, ref DynamicBuffer<Damage> damages, ref Health health) =>
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
        }
    }
}