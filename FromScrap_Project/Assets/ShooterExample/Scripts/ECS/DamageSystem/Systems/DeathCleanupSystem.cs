using DamageSystem.Components;
using Unity.Entities;

namespace DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class DeathCleanupSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer ecb = _ecbSystem.CreateCommandBuffer();

            Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();
            
        }
    }
}