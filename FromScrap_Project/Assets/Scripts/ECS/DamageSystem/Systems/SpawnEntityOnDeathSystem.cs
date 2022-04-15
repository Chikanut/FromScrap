using DamageSystem.Components;
using Unity.Entities;
using Unity.Transforms;

namespace DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateAfter(typeof(ResolveDamageSystem))]
    public partial class SpawnEntityOnDeathSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.WithAll<Dead>().ForEach((int entityInQueryIndex, in DynamicBuffer<SpawnEntityOnDeathBuffer> spawnEntityOnDeath, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < spawnEntityOnDeath.Length; i++)
                {
                    var spawnedEntity = ecb.Instantiate(entityInQueryIndex, spawnEntityOnDeath[i].SpawnEntity);
                   ecb.SetComponent(entityInQueryIndex, spawnedEntity, new Translation()
                   {
                       Value = localToWorld.Position
                   });
                }
            }).ScheduleParallel();
        }
    }
}