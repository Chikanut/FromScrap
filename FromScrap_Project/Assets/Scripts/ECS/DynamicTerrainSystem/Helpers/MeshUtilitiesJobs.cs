using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ECS.DynamicTerrainSystem.Helpers
{
    [BurstCompile]
    public struct CreateMeshColliderJob : IJob
    {
        [ReadOnly] public NativeArray<float3> MeshVerts;
        [ReadOnly] public NativeArray<int> MeshTris;
        [ReadOnly] public CollisionFilter CollisionFilter;
        public NativeArray<BlobAssetReference<Collider>> BlobCollider;

        public void Execute()
        {
            var cVerts = new NativeArray<float3>(MeshVerts.Length, Allocator.Temp);
            var cTris = new NativeArray<int3>(MeshTris.Length / 3, Allocator.Temp);

            for (var i = 0; i < MeshVerts.Length; i++)
                cVerts[i] = MeshVerts[i];

            var ii = 0;

            for (var j = 0; j < MeshTris.Length; j += 3)
                cTris[ii++] = new int3(MeshTris[j], MeshTris[j + 1], MeshTris[j + 2]);

            BlobCollider[0] = MeshCollider.Create(cVerts, cTris, CollisionFilter);

            cVerts.Dispose();
            cTris.Dispose();
        }
    }
}