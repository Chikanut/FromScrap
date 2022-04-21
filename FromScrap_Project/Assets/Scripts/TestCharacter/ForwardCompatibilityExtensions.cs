#if !UNITY_ENTITIES_0_12_OR_NEWER
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnsafeUtility_Collections = Unity.Collections.LowLevel.Unsafe.UnsafeUtility;

static class EntityCommandBufferExtensions
{
    public static EntityCommandBuffer.ParallelWriter AsParallelWriter(this EntityCommandBuffer commandBuffer) => commandBuffer.AsParallelWriter();
}

static class UnsafeUtility_ForwardCompatibility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ref T AsRef<T>(void* ptr) where T : struct => ref UnsafeUtilityEx.AsRef<T>(ptr);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Free(void* memory, Allocator allocator) => UnsafeUtility_Collections.Free(memory, allocator);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void* Malloc(long size, int alignment, Allocator allocator) => UnsafeUtility_Collections.Malloc(size, alignment, allocator);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void MemCpy(void* destination, void* source, long size) => UnsafeUtility_Collections.MemCpy(destination, source, size);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>() where T : struct => UnsafeUtility_Collections.SizeOf<T>();
}
#endif
