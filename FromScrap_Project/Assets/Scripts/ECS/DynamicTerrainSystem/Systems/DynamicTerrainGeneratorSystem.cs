using System.Collections.Generic;
using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainGeneratorSystem : SystemBase
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
            var playerPosition = float3.zero;

            Entities.ForEach((
                Entity entity,
                in CarIDComponent carIDComponent,
                in Translation translation
            ) =>
            {
                playerPosition = translation.Value;
            }).Run();

            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
                ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer
            ) =>
            {
                
                
                
                if (terrainTileBuffer.Length > 5)
                    terrainTileBuffer.RemoveAt(0);
            }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(this.Dependency);
        }
/*
        private static void SetupDynamicTerrainTiles(
            ref Entity generatorEntity,
            ref DynamicBuffer<DynamicTerrainTileInfoData> terrainTileBuffer,
            in DynamicTerrainBaseComponent dynamicTerrainBaseComponent,
            EntityCommandBuffer.ParallelWriter ecbs,
            float3 playerPos
        )
        {
            var playerTile = TileFromPosition(playerPos, dynamicTerrainBaseComponent.TerrainTileSize);
            var centerTiles = new List<Vector2>();
            
            centerTiles.Add(playerTile);
            
           
                List<GameObject> tileObjects = new List<GameObject>();
         
                foreach (Vector2 tile in centerTiles)
                {
                    bool isPlayerTile = tile == playerTile;
                    int radius = isPlayerTile ? radiusToRender : 1;
                    for (int i = -radius; i <= radius; i++)
                    for (int j = -radius; j <= radius; j++)
                        ActivateOrCreateTile((int) tile.X + i, (int) tile.Y + j, tileObjects);
                }
            
          
        }
        
        private static float2 TileFromPosition(float3 position, float3 terrainSize)
        {
            return new float2(Mathf.FloorToInt(position.x / terrainSize.x + .5f), Mathf.FloorToInt(position.z / terrainSize.z + .5f));
        }



      
      
       
       
        private int radiusToRender = 5;


        private void ActivateOrCreateTile(int xIndex, int yIndex, List<GameObject> tileObjects)
        {
            if (!terrainTiles.ContainsKey(new Vector2(xIndex, yIndex)))
            {
                tileObjects.Add(CreateTile(xIndex, yIndex));
            }
            else
            {
                GameObject t = terrainTiles[new Vector2(xIndex, yIndex)];
                tileObjects.Add(t);
                if (!t.activeSelf)
                    t.SetActive(true);
            }
        }

        private GameObject CreateTile(int xIndex, int yIndex)
        {
            var pos = new Vector3(terrainSize.x * xIndex, terrainSize.y, terrainSize.z * yIndex);
            GameObject terrain = Instantiate(
                terrainTilePrefab,
                pos,
                Quaternion.identity
            );
            terrain.name = TrimEnd(terrain.name, "(Clone)") + " [" + xIndex + " , " + yIndex + "]";

            terrainTiles.Add(new Vector2(xIndex, yIndex), terrain);

            GenerateMeshSimple gm = terrain.GetComponent<GenerateMeshSimple>();
            gm.TerrainSize = terrainSize;
            gm.Gradient = gradient;
            gm.NoiseScale = noiseScale;
            gm.CellSize = cellSize;
            gm.NoiseOffset = pos;
            gm.Generate();

            return terrain;
        }


        private static string TrimEnd(string str, string end)
        {
            if (str.EndsWith(end))
                return str.Substring(0, str.LastIndexOf(end));
            return str;
        }
      */
    }
    
}
