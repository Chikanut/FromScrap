using DamageSystem.Components;
using Unity.Entities;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    public partial class MoveShotSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            var ecbParallel = ecb.AsParallelWriter();
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((Entity e, ref Translation translation, ref ShotData shotData, in LocalToWorld localToWorld) =>
            {
                var forwardMovement = localToWorld.Forward * deltaTime * shotData.Velocity;
                translation.Value += forwardMovement;
                shotData.Lifetime -= deltaTime;

                if (shotData.Lifetime <= 0f)
                    ecbParallel.AddComponent<Dead>(e.Index, e);
            }).ScheduleParallel();
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}