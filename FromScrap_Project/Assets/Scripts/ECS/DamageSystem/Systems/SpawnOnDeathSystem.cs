using BovineLabs.Event.Systems;
using DamageSystem.Components;
using SpawnGameObjects.Components;
using Unity.Entities;
using Unity.Transforms;

namespace DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateAfter(typeof(ResolveDamageSystem))]
    public partial class SpawnOnDeathSystem : SystemBase
    {
        private EventSystem _eventSystem;
        protected override void OnCreate()
        {
            _eventSystem = this.World.GetOrCreateSystem<EventSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var writerEntityPoolEvent = _eventSystem.CreateEventWriter<SpawnEntityPoolObjectEvent>();
            Entities.WithAll<Dead>().ForEach((int entityInQueryIndex, in DynamicBuffer<SpawnEntityOnDeathBuffer> spawnEntityOnDeath, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < spawnEntityOnDeath.Length; i++)
                {
                    writerEntityPoolEvent.Write(new SpawnPoolObjectEvent(){Position = localToWorld.Position, SpawnObjectName = spawnEntityOnDeath[i].SpawnEntity});
                }
            }).ScheduleParallel();
            
            var writerPoolObjectPoolEvent = _eventSystem.CreateEventWriter<SpawnPoolObjectEvent>();
            
            Entities.WithAll<Dead>().ForEach((int entityInQueryIndex, in DynamicBuffer<SpawnPoolObjectOnDeathBuffer> spawnPoolObjectOnDeath, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < spawnPoolObjectOnDeath.Length; i++)
                {
                    writerPoolObjectPoolEvent.Write(new SpawnPoolObjectEvent(){Position = localToWorld.Position, SpawnObjectName = spawnPoolObjectOnDeath[i].SpawnObjectName});
                }
            }).ScheduleParallel();
            
            _eventSystem.AddJobHandleForProducer<SpawnPoolObjectEvent>(Dependency);
            _eventSystem.AddJobHandleForProducer<SpawnEntityPoolObjectEvent>(Dependency);
        }
    }
}