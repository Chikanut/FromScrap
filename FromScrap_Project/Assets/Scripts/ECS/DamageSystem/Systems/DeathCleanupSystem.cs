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
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter ();

            Dependency = Entities.WithoutBurst().WithAll<Dead>().ForEach((Entity entity, int entityInQueryIndex) =>
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel(Dependency);
            
            _ecbSystem.AddJobHandleForProducer (Dependency);
        }
    }
}