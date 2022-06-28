using BovineLabs.Event.Systems;
using ECS.GameResourcesSystem.Components;
using ECS.SignalSystems.Systems;
using Serializable.ResourcesManager;
using Unity.Entities;

namespace ECS.GameResourcesSystem.Systems
{
    public partial class GameResourcesSpawnLoadSystem : SystemBase
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
            var gameResourcesSpawnLoadEntity = Entity.Null;
            var gameResourcesSpawnEntity = Entity.Null;
            var gameResourcesSpawnEntityType = GameResourcesEntityTypeId.DynamicTerrain;
            var loadResourcesCompleteEventWriter = _eventSystem.CreateEventWriter<OnGameResourcesLoadedSignal>();

            Entities.ForEach((
                Entity entity,
                in GameResourcesLoadEntityId loadEntityId
            ) =>
            {
                gameResourcesSpawnLoadEntity = entity;
                gameResourcesSpawnEntity = loadEntityId.TargetEntity;
                gameResourcesSpawnEntityType = loadEntityId.TypeId;
            }).Run();

            Dependency = Entities.ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref GameResourcesSpawnComponent spawnComponent
            ) =>
            {
                if(spawnComponent.IsLoaded)
                    return;
                
                if(gameResourcesSpawnLoadEntity == Entity.Null)
                    return;
                
                if(gameResourcesSpawnEntity == Entity.Null)
                    return;

                if (gameResourcesSpawnEntityType == GameResourcesEntityTypeId.DynamicTerrain)
                {
                    spawnComponent.DynamicTerrainEntity = gameResourcesSpawnEntity;
                    ecbs.DestroyEntity(entityInQueryIndex, gameResourcesSpawnLoadEntity);
                }

                if (spawnComponent.DynamicTerrainEntity != Entity.Null)
                {
                    spawnComponent.IsLoaded = true;
                    loadResourcesCompleteEventWriter.Write(new OnGameResourcesLoadedSignal());
                }

            }).ScheduleParallel(Dependency);

            _eventSystem.AddJobHandleForProducer<OnGameResourcesLoadedSignal>(Dependency);
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}