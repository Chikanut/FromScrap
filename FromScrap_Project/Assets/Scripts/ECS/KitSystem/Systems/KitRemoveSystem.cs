using DamageSystem.Components;
using Kits.Components;
using Unity.Entities;

namespace Kits.Systems
{
    public partial class KitRemoveSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var kpkb = GetBufferFromEntity<KitPlatformKitsBuffer>();
            
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
          
            Dependency = Entities.WithNone<KitInstalatorComponent>().WithNone<Dead>().WithAll<KitRemoveComponent>().ForEach(
                (Entity entity, int entityInQueryIndex, KitComponent kitComponent) =>
                {
                    if (kpkb.HasComponent(kitComponent.Platform))
                    {
                        for (int i = 0; i < kpkb[kitComponent.Platform].Length; i++)
                        {
                            if (kpkb[kitComponent.Platform][i].ConnectedKit == entity)
                            {
                                kpkb[kitComponent.Platform].RemoveAt(i);
                            }
                        }
                    }

                    ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                }).Schedule(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}