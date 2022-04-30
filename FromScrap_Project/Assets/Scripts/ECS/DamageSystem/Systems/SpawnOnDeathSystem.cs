using BovineLabs.Event.Systems;
using DamageSystem.Components;
using SpawnGameObjects.Components;
using Unity.Entities;
using Unity.Transforms;

namespace DamageSystem.Systems
{
    [UpdateAfter(typeof(ResolveDamageSystem))]
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
                    writerEntityPoolEvent.Write(new SpawnEntityPoolObjectEvent(){Position = localToWorld.Position, SpawnChance = spawnEntityOnDeath[i].SpawnChance, EntityName = spawnEntityOnDeath[i].SpawnEntity});
                }
            }).ScheduleParallel();
            
            var writerPoolObjectPoolEvent = _eventSystem.CreateEventWriter<SpawnPoolObjectEvent>();
            
            Entities.WithAll<Dead>().ForEach((int entityInQueryIndex, in DynamicBuffer<SpawnPoolObjectOnDeathBuffer> spawnPoolObjectOnDeath, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < spawnPoolObjectOnDeath.Length; i++)
                {
                    writerPoolObjectPoolEvent.Write(new SpawnPoolObjectEvent(){Position = localToWorld.Position, SpawnChance = spawnPoolObjectOnDeath[i].SpawnChance, SpawnObjectName = spawnPoolObjectOnDeath[i].SpawnObjectName});
                }
            }).ScheduleParallel();
            
            var writerSoundObjectPoolEvent = _eventSystem.CreateEventWriter<SpawnSoundObjectEvent>();
            
            Entities.WithAll<Dead>().ForEach((int entityInQueryIndex, in DynamicBuffer<SpawnSoundObjectOnDeathBuffer> spawnSoundObjectOnDeath, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < spawnSoundObjectOnDeath.Length; i++)
                {
                    writerSoundObjectPoolEvent.Write(new SpawnSoundObjectEvent()
                    {
                       ClipName = spawnSoundObjectOnDeath[i].ClipName,
                       ClipType = spawnSoundObjectOnDeath[i].Type,
                       Delay = spawnSoundObjectOnDeath[i].Delay,
                       Pitch = spawnSoundObjectOnDeath[i].Pitch,
                       PitchTime = spawnSoundObjectOnDeath[i].PitchTime,
                       Position = localToWorld.Position
                    });
                }
            }).ScheduleParallel();
            
            _eventSystem.AddJobHandleForProducer<SpawnSoundObjectEvent>(Dependency);
            _eventSystem.AddJobHandleForProducer<SpawnPoolObjectEvent>(Dependency);
            _eventSystem.AddJobHandleForProducer<SpawnEntityPoolObjectEvent>(Dependency);
        }
    }
}