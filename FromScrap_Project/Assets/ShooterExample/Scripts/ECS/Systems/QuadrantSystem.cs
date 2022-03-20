using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public struct QuadrantData : IComponentData {
    public Entity entity;
    public float3 position;
    public QuadrantEntity quadrantEntity;
}

public partial class QuadrantSystem : SystemBase
{
    public static NativeMultiHashMap<int, QuadrantData> QuadrantDataHashMap
    {

        get
        {
            var arrayLength = QuadrantSystem.quadrantDataHashMap.Capacity;
            var quadrantDataHashMap = new NativeMultiHashMap<int, QuadrantData>(arrayLength, Allocator.TempJob);
            var copyJob =
                new CopyNativeHashMapJob(QuadrantSystem.quadrantDataHashMap, quadrantDataHashMap);
            copyJob.Run(arrayLength);

            return quadrantDataHashMap;
        }
    }

    private static NativeMultiHashMap<int, QuadrantData> quadrantDataHashMap;

    [BurstCompile]
    public struct CopyNativeHashMapJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion] [ReadOnly] private NativeArray<int> Keys;

        [DeallocateOnJobCompletion] [ReadOnly] private NativeArray<QuadrantData> Values;

        [WriteOnly] private NativeMultiHashMap<int, QuadrantData>.ParallelWriter OutputWriter;

        public CopyNativeHashMapJob(NativeMultiHashMap<int, QuadrantData> input,
            NativeMultiHashMap<int, QuadrantData> output)
        {

            Keys = input.GetKeyArray(Allocator.TempJob);
            Values = input.GetValueArray(Allocator.TempJob);

            output.Clear();
            OutputWriter = output.AsParallelWriter();
        }

        public void Execute(int index)
        {
            OutputWriter.Add(Keys[index], Values[index]);
        }
    }

    public const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 25;

    private static int GetPositionHashMapKey(float3 position)
    {
        return (int) (math.floor(position.x / quadrantCellSize) +
                      (quadrantYMultiplier * math.floor(position.y / quadrantCellSize)));
    }

    protected override void OnCreate()
    {
        quadrantDataHashMap = new NativeMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
        base.OnCreate();
    }

    protected override void OnDestroy()
    {
        quadrantDataHashMap.Dispose();
        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        var entityQuery = GetEntityQuery(typeof(QuadrantEntity), typeof(Translation));

        quadrantDataHashMap.Clear();
        if (entityQuery.CalculateEntityCount() > quadrantDataHashMap.Capacity)
            quadrantDataHashMap.Capacity = entityQuery.CalculateEntityCount();

        var nativeMultiHashMap = quadrantDataHashMap.AsParallelWriter();

        Dependency = Entities.WithAll<QuadrantEntity, Translation>().WithChangeFilter<Translation>().ForEach(
            (Entity entity, ref Translation translation, ref QuadrantEntity quadrantEntity) =>
            {
                var hashMapKey = GetPositionHashMapKey(translation.Value);
                quadrantEntity.HashKey = hashMapKey;
                nativeMultiHashMap.Add(hashMapKey, new QuadrantData
                {
                    entity = entity,
                    position = translation.Value,
                    quadrantEntity = quadrantEntity,
                });
            }).ScheduleParallel(Dependency);
        Dependency.Complete();
    }
}

