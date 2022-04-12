using DamageSystem.Components;
using Unity.Entities;
using Unity.Transforms;

namespace DamageSystem.Systems
{
    [UpdateAfter(typeof(ResolveDamageSystem))]
    public partial class SpawnEntityOnDeathSystem : SystemBase
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
            
            Entities.WithAll<Dead>().ForEach((in DynamicBuffer<SpawnEntityOnDeathBuffer> spawnEntityOnDeath, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < spawnEntityOnDeath.Length; i++)
                {
                    var spawnedEntity = ecb.Instantiate(spawnEntityOnDeath[i].SpawnEntity);
                   ecb.SetComponent(spawnedEntity, new Translation()
                   {
                       Value = localToWorld.Position
                   });
                }
            }).Run();
        }
    }
}