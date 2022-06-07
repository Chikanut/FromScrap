using BovineLabs.Event.Systems;
using DamageSystem.Components;
using ECS.SignalSystems.Systems;
using Kits.Components;
using Unity.Entities;

namespace Kits.Systems
{
    public partial class KitRemoveSystem : SystemBase
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
            var kpkb = GetBufferFromEntity<KitPlatformKitsBuffer>();
            var kpc = GetComponentDataFromEntity<KitPlatformComponent>();
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            var installEvent = _eventSystem.CreateEventWriter<KitInstalledSignal>();
            
            Dependency = Entities.WithNone<KitInstalatorComponent>().WithNone<Dead>().WithAll<KitRemoveComponent>().ForEach(
                (Entity entity, int entityInQueryIndex, in KitComponent kitComponent) =>
                {
                    if (kpkb.HasComponent(kitComponent.Platform))
                    {
                        for (int i = 0; i < kpkb[kitComponent.Platform].Length; i++)
                        {
                            if (kpkb[kitComponent.Platform][i].ConnectedKit == entity)
                            {
                                kpkb[kitComponent.Platform].RemoveAt(i);
                            }

                            if (kpc.HasComponent(kitComponent.Platform))
                            {
                                installEvent.Write(new KitInstalledSignal() {Car = kpc[kitComponent.Platform].Scheme});
                            }
                        }
                    }

                    ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                }).Schedule(Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
            _eventSystem.AddJobHandleForProducer<KitInstalledSignal>(Dependency);
        }
    }
}