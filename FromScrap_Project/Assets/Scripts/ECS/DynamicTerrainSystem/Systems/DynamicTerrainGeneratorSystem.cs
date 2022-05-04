using System.Collections.Generic;
using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Random = Unity.Mathematics.Random;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainGeneratorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
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






        private GameObject terrainTilePrefab = null;
        private Vector3 terrainSize = new Vector3(20, 1, 20);
        private Gradient gradient;
        private float noiseScale = 3, cellSize = 1;
        private int radiusToRender = 5;
        private Transform[] gameTransforms;
        private Transform playerTransform;
        private Vector2 startOffset;
        private Dictionary<Vector2, GameObject> terrainTiles = new Dictionary<Vector2, GameObject>();
        private Vector2[] previousCenterTiles;
        private List<GameObject> previousTileObjects = new List<GameObject>();


        public void InitialLoad()
        {
            DestroyTerrain();

            //choose a place on perlin noise (which loops after 256)
            startOffset = new Vector2(Random.Range(0f, 256f), Random.Range(0f, 256f));
        }

        private void Update()
        {
            //save the tile the player is on
            Vector2 playerTile = TileFromPosition(playerTransform.position);
            //save the tiles of all tracked objects in gameTransforms (including the player)
            List<Vector2> centerTiles = new List<Vector2>();
            centerTiles.Add(playerTile);
            foreach (Transform t in gameTransforms)
                centerTiles.Add(TileFromPosition(t.position));

            //if no tiles exist yet or tiles should change
            if (previousCenterTiles == null || HaveTilesChanged(centerTiles))
            {
                List<GameObject> tileObjects = new List<GameObject>();
                //activate new tiles
                foreach (Vector2 tile in centerTiles)
                {
                    bool isPlayerTile = tile == playerTile;
                    int radius = isPlayerTile ? radiusToRender : 1;
                    for (int i = -radius; i <= radius; i++)
                    for (int j = -radius; j <= radius; j++)
                        ActivateOrCreateTile((int) tile.x + i, (int) tile.y + j, tileObjects);
                }

                //deactivate old tiles
                foreach (GameObject g in previousTileObjects)
                    if (!tileObjects.Contains(g))
                        g.SetActive(false);

                previousTileObjects = new List<GameObject>(tileObjects);
            }

            previousCenterTiles = centerTiles.ToArray();
        }

        //Helper methods below

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
            GameObject terrain = Instantiate(
                terrainTilePrefab,
                new Vector3(terrainSize.x * xIndex, terrainSize.y, terrainSize.z * yIndex),
                Quaternion.identity
            );
            terrain.name = TrimEnd(terrain.name, "(Clone)") + " [" + xIndex + " , " + yIndex + "]";

            terrainTiles.Add(new Vector2(xIndex, yIndex), terrain);

            GenerateMeshSimple gm = terrain.GetComponent<GenerateMeshSimple>();
            gm.TerrainSize = terrainSize;
            gm.Gradient = gradient;
            gm.NoiseScale = noiseScale;
            gm.CellSize = cellSize;
            gm.NoiseOffset = NoiseOffset(xIndex, yIndex);
            gm.Generate();

            return terrain;
        }

        private Vector2 NoiseOffset(int xIndex, int yIndex)
        {
            Vector2 noiseOffset = new Vector2(
                (xIndex * noiseScale + startOffset.x) % 256,
                (yIndex * noiseScale + startOffset.y) % 256
            );
            //account for negatives (ex. -1 % 256 = -1). needs to loop around to 255
            if (noiseOffset.x < 0)
                noiseOffset = new Vector2(noiseOffset.x + 256, noiseOffset.y);
            if (noiseOffset.y < 0)
                noiseOffset = new Vector2(noiseOffset.x, noiseOffset.y + 256);
            return noiseOffset;
        }

        private Vector2 TileFromPosition(Vector3 position)
        {
            return new Vector2(Mathf.FloorToInt(position.x / terrainSize.x + .5f),
                Mathf.FloorToInt(position.z / terrainSize.z + .5f));
        }

        private bool HaveTilesChanged(List<Vector2> centerTiles)
        {
            if (previousCenterTiles.Length != centerTiles.Count)
                return true;
            for (int i = 0; i < previousCenterTiles.Length; i++)
                if (previousCenterTiles[i] != centerTiles[i])
                    return true;
            return false;
        }

        public void DestroyTerrain()
        {
            foreach (KeyValuePair<Vector2, GameObject> kv in terrainTiles)
                Destroy(kv.Value);
            terrainTiles.Clear();
        }

        private static string TrimEnd(string str, string end)
        {
            if (str.EndsWith(end))
                return str.Substring(0, str.LastIndexOf(end));
            return str;
        }
    }
}
