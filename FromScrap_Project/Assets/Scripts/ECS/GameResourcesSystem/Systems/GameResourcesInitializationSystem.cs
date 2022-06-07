using Unity.Entities;

namespace ECS.GameResourcesSystem.Systems
{
    public partial class GameResourcesInitializationSystem  : SystemBase
    {
        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecbs = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Dependency = Entities.ForEach((
                Entity entity 
            ) =>
            {
                
            }).ScheduleParallel(Dependency);
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
        
        private static void GenerateTerrainTile(
            ref Entity generatorEntity,
            EntityCommandBuffer.ParallelWriter ecbs,
            int entityInQueryIndex,
            int index
        )
        {
            /*
        var tileEntity = ecbs.Instantiate(entityInQueryIndex, terrainComponent.TerrainTileEntity);
        
        ecbs.AddComponent(entityInQueryIndex, tileEntity, translation);
     
        ecbs.AppendToBuffer(entityInQueryIndex, generatorEntity, new DynamicTerrainTileInfoData()
        {
            TileEntity = tileEntity,
            TileIndex = tileData.TileIndex,
            TileState = DynamicTerrainTileState.IsGenerated
        });
        */
        }
    }
}