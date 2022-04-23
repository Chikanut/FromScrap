using DamageSystem.Components;
using Unity.Entities;

namespace DamageSystem.Systems
{
    [UpdateAfter(typeof(DamageCollisionSystem))]
    public partial class CleanUpDamageDealersSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            
            Dependency = Entities.WithNone<Dead>().ForEach((Entity entity, int entityInQueryIndex, in DealDamage dealDamage) =>
            {
                if (dealDamage.MaxHits != 0 && dealDamage.CurrentHit >= dealDamage.MaxHits)
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                }
            }).Schedule(Dependency);;
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}