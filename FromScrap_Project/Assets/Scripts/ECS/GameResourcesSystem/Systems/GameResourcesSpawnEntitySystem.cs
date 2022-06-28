using BovineLabs.Event.Systems;
using ECS.GameResourcesSystem.Components;
using ECS.SignalSystems.Systems;
using Serializable.ResourcesManager;
using Unity.Entities;

namespace ECS.GameResourcesSystem.Systems
{
    public partial class GameResourcesSpawnEntitySystem  : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;
        private EventSystem _eventSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            _eventSystem = this.World.GetOrCreateSystem<EventSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbs = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var spawnerEntity = Entity.Null;
            var spawnEntityTypeId = GameResourcesEntityTypeId.DynamicTerrain;
            var spawnResourcesCompleteEventWriter = _eventSystem.CreateEventWriter<OnGameResourcesSpawnedSignal>();
            
            Entities.ForEach((
                Entity entity,
                in GameResourcesSpawnEntityId entityTypeId
            ) =>
            {
                spawnerEntity = entity;
                spawnEntityTypeId = entityTypeId.TypeId;
            }).Run();
            
            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref GameResourcesSpawnComponent spawnComponent
            ) =>
            {
                if(!spawnComponent.IsLoaded)
                    return;
                
                if(spawnComponent.IsSpawned)
                    return;
                
                if(spawnerEntity == Entity.Null)
                    return;

                if (spawnEntityTypeId == GameResourcesEntityTypeId.DynamicTerrain)
                {
                    spawnComponent.IsSpawned = true;
                    ecbs.Instantiate(entityInQueryIndex, spawnComponent.DynamicTerrainEntity);
                    ecbs.DestroyEntity(entityInQueryIndex, spawnerEntity);
                    spawnResourcesCompleteEventWriter.Write(new OnGameResourcesSpawnedSignal());
                }
            }).ScheduleParallel(Dependency);

            _eventSystem.AddJobHandleForProducer<OnGameResourcesSpawnedSignal>(Dependency);
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}