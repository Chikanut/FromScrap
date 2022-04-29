using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ECS.DynamicTerrainSystem
{
    public partial class DynamicTerrainTileSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
        private EntityCommandBufferSystem entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
            var ecbs = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.ForEach((
                Entity entity,
                ref DynamicTerrainTileComponent tileComponent
               
            ) =>
            {
                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                var isUpdated = tileComponent.IsUpdated;
                
                if(!isUpdated)
                    Generate(ref renderMesh, ref tileComponent);
                
                //ecb.SetSharedComponent(entity, renderMesh);
                ecbs.SetSharedComponent(0, entity, renderMesh);
                
                

            }).WithoutBurst().Run();
        }

        public static void Generate(ref RenderMesh renderMesh, ref DynamicTerrainTileComponent tileComponent)
        {
            var TerrainSize = tileComponent.TerrainSize;
            var CellSize = tileComponent.CellSize;
            var NoiseOffset = tileComponent.NoiseOffset;
            var NoiseScale = tileComponent.NoiseScale;
            var Gradient = tileComponent.Gradient;
            
            var draft = TerrainDraft(TerrainSize, CellSize, NoiseOffset, NoiseScale, Gradient);
            
            draft.Move(Vector3.left * TerrainSize.x / 2 + Vector3.back * TerrainSize.z / 2);
            //renderMesh.mesh = draft.ToMesh();
            renderMesh.mesh.SetVertexBufferData(draft.ToMesh().vertices, 0, 0, draft.ToMesh().vertices.Length, 0, MeshUpdateFlags.Default);
            renderMesh.mesh.RecalculateBounds(MeshUpdateFlags.Default);
            renderMesh.mesh.RecalculateNormals(MeshUpdateFlags.Default);
            
            var vertices = renderMesh.mesh.vertices;
            var normals = renderMesh.mesh.normals;
            for (var i = 0; i < vertices.Length; i++)
            {
                normals[i] = Vector3.Lerp(normals[i], vertices[i].normalized, tileComponent.NormalsSmoothPower);
            }
            renderMesh.mesh.normals = normals;

            //todo: generate mesh collider
            /*
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            if (meshCollider)
                meshCollider.sharedMesh = meshFilter.mesh;
            */

            tileComponent.IsUpdated = true;
        }

        private static MeshDraft TerrainDraft(Vector3 terrainSize, float cellSize, Vector2 noiseOffset,
            float noiseScale, float gradient)
        {
            int xSegments = Mathf.FloorToInt(terrainSize.x / cellSize);
            int zSegments = Mathf.FloorToInt(terrainSize.z / cellSize);

            float xStep = terrainSize.x / xSegments;
            float zStep = terrainSize.z / zSegments;
            int vertexCount = 6 * xSegments * zSegments;
            MeshDraft draft = new MeshDraft
            {
                name = "Terrain",
                vertices = new List<Vector3>(vertexCount),
                triangles = new List<int>(vertexCount),
                normals = new List<Vector3>(vertexCount),
                colors = new List<Color>(vertexCount)
            };

            for (int i = 0; i < vertexCount; i++)
            {
                draft.vertices.Add(Vector3.zero);
                draft.triangles.Add(0);
                draft.normals.Add(Vector3.zero);
                draft.colors.Add(Color.black);
            }

            for (int x = 0; x < xSegments; x++)
            {
                for (int z = 0; z < zSegments; z++)
                {
                    int index0 = 6 * (x + z * xSegments);
                    int index1 = index0 + 1;
                    int index2 = index0 + 2;
                    int index3 = index0 + 3;
                    int index4 = index0 + 4;
                    int index5 = index0 + 5;

                    float height00 = GetHeight(x + 0, z + 0, xSegments, zSegments, noiseOffset, noiseScale);
                    float height01 = GetHeight(x + 0, z + 1, xSegments, zSegments, noiseOffset, noiseScale);
                    float height10 = GetHeight(x + 1, z + 0, xSegments, zSegments, noiseOffset, noiseScale);
                    float height11 = GetHeight(x + 1, z + 1, xSegments, zSegments, noiseOffset, noiseScale);

                    Vector3 vertex00 = new Vector3((x + 0) * xStep, height00 * terrainSize.y, (z + 0) * zStep);
                    Vector3 vertex01 = new Vector3((x + 0) * xStep, height01 * terrainSize.y, (z + 1) * zStep);
                    Vector3 vertex10 = new Vector3((x + 1) * xStep, height10 * terrainSize.y, (z + 0) * zStep);
                    Vector3 vertex11 = new Vector3((x + 1) * xStep, height11 * terrainSize.y, (z + 1) * zStep);

                    draft.vertices[index0] = vertex00;
                    draft.vertices[index1] = vertex01;
                    draft.vertices[index2] = vertex11;
                    draft.vertices[index3] = vertex00;
                    draft.vertices[index4] = vertex11;
                    draft.vertices[index5] = vertex10;

                    draft.colors[index0] = new Color(gradient * height00, gradient * height00, gradient * height00, 1f);
                    draft.colors[index1] = new Color(gradient * height01, gradient * height01, gradient * height01, 1f); 
                    draft.colors[index2] = new Color(gradient * height11, gradient * height11, gradient * height11, 1f);
                    draft.colors[index3] = new Color(gradient * height00, gradient * height00, gradient * height00, 1f);
                    draft.colors[index4] = new Color(gradient * height11, gradient * height11, gradient * height11, 1f);
                    draft.colors[index5] = new Color(gradient * height10, gradient * height10, gradient * height10, 1f);
                    Vector3 normal000111 = Vector3.Cross(vertex01 - vertex00, vertex11 - vertex00).normalized;
                    Vector3 normal001011 = Vector3.Cross(vertex11 - vertex00, vertex10 - vertex00).normalized;

                    draft.normals[index0] = normal000111;
                    draft.normals[index1] = normal000111;
                    draft.normals[index2] = normal000111;
                    draft.normals[index3] = normal001011;
                    draft.normals[index4] = normal001011;
                    draft.normals[index5] = normal001011;

                    draft.triangles[index0] = index0;
                    draft.triangles[index1] = index1;
                    draft.triangles[index2] = index2;
                    draft.triangles[index3] = index3;
                    draft.triangles[index4] = index4;
                    draft.triangles[index5] = index5;
                }
            }

            return draft;
        }
        
        private static float GetHeight(int x, int z, int xSegments, int zSegments, Vector2 noiseOffset, float noiseScale) {
            float noiseX = noiseScale * x / xSegments + noiseOffset.x;
            float noiseZ = noiseScale * z / zSegments + noiseOffset.y;
            return Mathf.PerlinNoise(noiseX, noiseZ);
            //return 1f;
        }
    }
}
    