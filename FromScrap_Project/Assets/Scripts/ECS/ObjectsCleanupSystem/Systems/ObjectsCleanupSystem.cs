using IsVisible.Components;
using Lifetime.Components;
using Unity.Entities;

namespace ObjectsCleanup.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ObjectsCleanupSystem : SystemBase
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
            Entities.ForEach((Entity entity, in IsVisibleComponent isVisible, in LifetimeComponent lifetime) =>
            {
                if(lifetime.CurrentLifetime > lifetime.MaxLifeTime && !isVisible.Value)
                    ecb.DestroyEntity(entity);
            }).Run();
        }
    }
}