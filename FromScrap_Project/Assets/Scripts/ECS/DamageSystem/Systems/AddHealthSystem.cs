using Collectables.Components;
using DamageSystem.Components;
using Unity.Entities;
namespace DamageSystem.Systems
{
    [UpdateBefore(typeof(ResolveDamageSystem))]
    public partial class AddHealthSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var health = GetComponentDataFromEntity<Health>();
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, in AddHealthComponent addHealth, in CollectableGatheredComponent collectInfo) =>
            {
                if (!health.HasComponent(collectInfo.CollectedEntity)) return;

                var h = health[collectInfo.CollectedEntity];
                h.AddHealth(addHealth.Value);
                health[collectInfo.CollectedEntity] = h;
                
                ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                    
            }).Schedule(Dependency);

            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}