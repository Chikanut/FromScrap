using System.Collections.Generic;
using ECS.DynamicTerrainSystem.Helpers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
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
                ref DynamicTerrainTileComponent tileComponent,
                ref RenderBounds renderBounds
            ) =>
            {
                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                var isUpdated = tileComponent.IsUpdated;

                if (!isUpdated)
                {
                    Generate(ref renderMesh, ref tileComponent, ref renderBounds);

                    //ecb.SetSharedComponent(entity, renderMesh);
                    ecbs.SetSharedComponent(0, entity, renderMesh);
                   
                    var blobCollider =
                        new NativeArray<BlobAssetReference<Unity.Physics.Collider>>(1, Allocator.TempJob);
                    var nVerts = new NativeArray<Vector3>(renderMesh.mesh.vertices, Allocator.TempJob)
                        .Reinterpret<float3>();
                    var nTris = new NativeArray<int>(renderMesh.mesh.triangles, Allocator.TempJob);

                    var cmcj = new CreateMeshColliderJob
                        {MeshVerts = nVerts, MeshTris = nTris, BlobCollider = blobCollider};
                    cmcj.Run();

                    ecbs.AddComponent(0, entity, new PhysicsCollider {Value = blobCollider[0]});

                    nVerts.Dispose();
                    nTris.Dispose();
                    blobCollider.Dispose();
                }

            }).WithoutBurst().Run();
        }
        
        [BurstCompile]
        public struct CreateMeshColliderJob : IJob {
 
            [ReadOnly] public NativeArray<float3> MeshVerts;
            [ReadOnly] public NativeArray<int> MeshTris;
            public NativeArray<BlobAssetReference<Unity.Physics.Collider>> BlobCollider;
 
            public void Execute() {
 
                NativeArray<float3> CVerts = new NativeArray<float3>(MeshVerts.Length, Allocator.Temp);
                NativeArray<int3> CTris = new NativeArray<int3>(MeshTris.Length / 3, Allocator.Temp);
 
                for (int i = 0; i < MeshVerts.Length; i++) { CVerts[i] = MeshVerts[i]; }
                int ii = 0;
                for (int j = 0; j < MeshTris.Length; j += 3) {
                    CTris[ii++] = new int3(MeshTris[j], MeshTris[j + 1], MeshTris[j + 2]);
                }
 
                CollisionFilter Filter = new CollisionFilter { BelongsTo = 1, CollidesWith = 1 << 1, GroupIndex = 0 };
 
                BlobCollider[0] = Unity.Physics.MeshCollider.Create(CVerts, CTris);
                CVerts.Dispose();
                CTris.Dispose();
            }
        }

        private static void Generate(
            ref RenderMesh renderMesh, 
            ref DynamicTerrainTileComponent tileComponent,
            ref RenderBounds renderBounds)
        {
            var terrainSize = tileComponent.TerrainSize;
            var cellSize = tileComponent.CellSize;
            var tile = TileFromPosition(tileComponent.NoiseOffset, terrainSize);
            var noiseScale = tileComponent.NoiseScale;
            var noiseOffset = NoiseOffset(tile.x, tile.y, noiseScale);
            var gradient = tileComponent.Gradient;
            var enableVertexColors = tileComponent.IsVertexColorsEnabled;
            var uvMapCalculator = new UVMapCalculator();
            var normalsCalculator = new MeshNormalsCalculator();
            var draft = TerrainDraft(terrainSize, cellSize, noiseOffset, noiseScale, gradient, enableVertexColors);
         
            draft.Move(Vector3.left * terrainSize.x / 2 + Vector3.back * terrainSize.z / 2);
            renderMesh.mesh = draft.ToMesh();
            renderMesh.mesh.SetUVs(tileComponent.UVMapChannel, uvMapCalculator.CalculatedUVs(renderMesh.mesh.vertices, tileComponent.UVMapScale));
            renderMesh.mesh.normals = normalsCalculator.RecalculatedNormals(renderMesh.mesh, tileComponent.NormalsSmoothAngle);
          
            CalculateRenderBounds(ref tileComponent, ref renderBounds);
            
            tileComponent.IsUpdated = true;
        }
        
        private static void CalculateRenderBounds(
            ref DynamicTerrainTileComponent tileComponent,
            ref RenderBounds renderBounds)
        {
            var x = tileComponent.TerrainSize.x / 2f;
            var z = tileComponent.TerrainSize.z / 2f;
            var newValue = new AABB()
            {
                Center = renderBounds.Value.Center,
                Extents = new float3(x, renderBounds.Value.Extents.y, z)
            };

            renderBounds.Value = newValue;
        }

        private static MeshDraft TerrainDraft(
            Vector3 terrainSize, 
            float cellSize, 
            Vector2 noiseOffset,
            float noiseScale, 
            float gradient, 
            bool enableVertexColors)
        {
            var xSegments = Mathf.FloorToInt(terrainSize.x / cellSize);
            var zSegments = Mathf.FloorToInt(terrainSize.z / cellSize);

            var xStep = terrainSize.x / xSegments;
            var zStep = terrainSize.z / zSegments;
            var vertexCount = 6 * xSegments * zSegments;
            var draft = new MeshDraft
            {
                name = "TerrainTile",
                vertices = new List<Vector3>(vertexCount),
                triangles = new List<int>(vertexCount),
                normals = new List<Vector3>(vertexCount),
                colors = new List<Color>(vertexCount)
            };

            for (var i = 0; i < vertexCount; i++)
            {
                draft.vertices.Add(Vector3.zero);
                draft.triangles.Add(0);
                draft.normals.Add(Vector3.zero);
                draft.colors.Add(Color.black);
            }

            for (var x = 0; x < xSegments; x++)
            {
                for (var z = 0; z < zSegments; z++)
                {
                    var index0 = 6 * (x + z * xSegments);
                    var index1 = index0 + 1;
                    var index2 = index0 + 2;
                    var index3 = index0 + 3;
                    var index4 = index0 + 4;
                    var index5 = index0 + 5;

                    var height00 = GetHeight(x + 0, z + 0, xSegments, zSegments, noiseOffset, noiseScale);
                    var height01 = GetHeight(x + 0, z + 1, xSegments, zSegments, noiseOffset, noiseScale);
                    var height10 = GetHeight(x + 1, z + 0, xSegments, zSegments, noiseOffset, noiseScale);
                    var height11 = GetHeight(x + 1, z + 1, xSegments, zSegments, noiseOffset, noiseScale);

                    var vertex00 = new Vector3((x + 0) * xStep, height00 * terrainSize.y, (z + 0) * zStep);
                    var vertex01 = new Vector3((x + 0) * xStep, height01 * terrainSize.y, (z + 1) * zStep);
                    var vertex10 = new Vector3((x + 1) * xStep, height10 * terrainSize.y, (z + 0) * zStep);
                    var vertex11 = new Vector3((x + 1) * xStep, height11 * terrainSize.y, (z + 1) * zStep);

                    draft.vertices[index0] = vertex00;
                    draft.vertices[index1] = vertex01;
                    draft.vertices[index2] = vertex11;
                    draft.vertices[index3] = vertex00;
                    draft.vertices[index4] = vertex11;
                    draft.vertices[index5] = vertex10;

                    if (enableVertexColors)
                    {
                        draft.colors[index0] = new Color(gradient * height00, gradient * height00, gradient * height00,
                            1f);
                        draft.colors[index1] = new Color(gradient * height01, gradient * height01, gradient * height01,
                            1f);
                        draft.colors[index2] = new Color(gradient * height11, gradient * height11, gradient * height11,
                            1f);
                        draft.colors[index3] = new Color(gradient * height00, gradient * height00, gradient * height00,
                            1f);
                        draft.colors[index4] = new Color(gradient * height11, gradient * height11, gradient * height11,
                            1f);
                        draft.colors[index5] = new Color(gradient * height10, gradient * height10, gradient * height10,
                            1f);
                    }

                    var normal000111 = Vector3.Cross(vertex01 - vertex00, vertex11 - vertex00).normalized;
                    var normal001011 = Vector3.Cross(vertex11 - vertex00, vertex10 - vertex00).normalized;

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

        private static float GetHeight(int x, int z, int xSegments, int zSegments, Vector2 noiseOffset,
            float noiseScale)
        {
            var noiseX = noiseScale * x / xSegments + noiseOffset.x;
            var noiseZ = noiseScale * z / zSegments + noiseOffset.y;

            return Mathf.PerlinNoise(noiseX, noiseZ);
        }
        
        private static Vector2 NoiseOffset(float xIndex, float yIndex, float noiseScale) {
            Vector2 noiseOffset = new Vector2(
                (xIndex * noiseScale) % 256,
                (yIndex * noiseScale) % 256
            );
            //account for negatives (ex. -1 % 256 = -1). needs to loop around to 255
            if (noiseOffset.x < 0)
                noiseOffset = new Vector2(noiseOffset.x + 256, noiseOffset.y);
            if (noiseOffset.y < 0)
                noiseOffset = new Vector2(noiseOffset.x, noiseOffset.y + 256);
            return noiseOffset;
        }
        
        private static Vector2 TileFromPosition(float2 position, float3 terrainSize) {
            return new Vector2(Mathf.FloorToInt(position.x / terrainSize.x + .5f), Mathf.FloorToInt(position.y / terrainSize.z + .5f));
        }
    }
}
    