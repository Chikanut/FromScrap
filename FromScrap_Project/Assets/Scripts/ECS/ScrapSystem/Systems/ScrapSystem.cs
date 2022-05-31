using BovineLabs.Event.Systems;
using Collectables.Components;
using DamageSystem.Components;
using Scrap.Components;
using Unity.Entities;

namespace Scrap.Systems
{
    public partial class ScrapSystem : SystemBase
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
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            var gatherEvent = _eventSystem.CreateEventWriter<ScrapGatherSignal>();

            Dependency = Entities.WithAll<CollectableGatheredComponent>().ForEach((Entity entity, int entityInQueryIndex, in ScrapComponent scrap) =>
            {
                gatherEvent.Write(new ScrapGatherSignal(){Value = scrap.Value});
                ecb.AddComponent(entityInQueryIndex, entity, new Dead());
                    
            }).Schedule(Dependency);

            _ecbSystem.AddJobHandleForProducer(Dependency);
            _eventSystem.AddJobHandleForProducer<ScrapGatherSignal>(Dependency);
        }
    }
}