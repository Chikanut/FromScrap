using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainGeneratorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var playerPosition = float3.zero;
            /*
            Entities.ForEach((
                Entity entity,
                in CarIDComponent carIDComponent,
                in Translation translation
            ) =>
            {
                playerPosition = translation.Value;
            }).Run();
            */
            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer
            ) =>
            {
                var newTile = new DynamicTerrainTileInfoData()
                {
                    TileIndex = new float2(playerPosition.x, playerPosition.y),
                    TileEntity = entity
                };

                terrainTileBuffer.Add(newTile);
        
                if (terrainTileBuffer.Length > 5)
                    terrainTileBuffer.RemoveAt(0);
            }).ScheduleParallel();
        } 
    }
}
