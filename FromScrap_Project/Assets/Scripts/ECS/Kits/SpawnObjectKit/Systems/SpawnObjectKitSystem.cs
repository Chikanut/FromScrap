using DamageSystem.Components;
using Kits.Components;
using SpawnObjectKit.Components;
using Unity.Entities;

namespace SpawnObjectKit.Systems
{
    public partial class SpawnObjectKitSystem : SystemBase
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
            
            Dependency = Entities.WithNone<KitInstalatorComponent>().WithNone<Dead>().WithNone<KitRemoveComponent>().WithAll<SpawnObjectKitComponent>().ForEach(
                (Entity entity, int entityInQueryIndex) =>
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new KitRemoveComponent());
                }).Schedule(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}