using DamageSystem.Components;
using Kits.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Kits.Systems
{
    public partial class KitCleanupSystem : SystemBase
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
            
            Dependency = Entities.WithNone<KitInstalatorComponent>().WithNone<Dead>().WithNone<Parent>().WithAll<KitComponent>().ForEach(
                (Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                }).Schedule(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}