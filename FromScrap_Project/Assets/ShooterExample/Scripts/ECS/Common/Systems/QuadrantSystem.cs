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
    public const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 50;
    
    public NativeMultiHashMap<int, QuadrantData> QuadrantDataHashMap;
    public JobHandle CurrentHandle;
    
    private static int GetPositionHashMapKey(float3 position)
    {
        return (int) (math.floor(position.x / quadrantCellSize) +
                      (quadrantYMultiplier * math.floor(position.z / quadrantCellSize)));
    }

    private EntityQuery TargetEntityQuery;

    protected override void OnCreate()
    {
        QuadrantDataHashMap = new NativeMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
        TargetEntityQuery = GetEntityQuery(typeof(QuadrantEntity), typeof(Translation));
        base.OnCreate();
    }

    protected override void OnDestroy()
    {
        QuadrantDataHashMap.Dispose();
        base.OnDestroy();
    }
    
    protected override void OnUpdate()
    {
        QuadrantDataHashMap.Clear();
        
        if (TargetEntityQuery.CalculateEntityCount() > QuadrantDataHashMap.Capacity)
            QuadrantDataHashMap.Capacity = TargetEntityQuery.CalculateEntityCount();

        var nativeMultiHashMap = QuadrantDataHashMap.AsParallelWriter();

        CurrentHandle = Entities.WithAll<QuadrantEntity, Translation>().ForEach(
            (Entity entity, ref QuadrantEntity quadrantEntity, in LocalToWorld translation) =>
            {
                var hashMapKey = GetPositionHashMapKey(translation.Position);
                quadrantEntity.HashKey = hashMapKey;
                nativeMultiHashMap.Add(hashMapKey, new QuadrantData
                {
                    entity = entity,
                    position = translation.Position,
                    quadrantEntity = quadrantEntity,
                });
            }).ScheduleParallel(Dependency);

        Dependency = JobHandle.CombineDependencies(Dependency, CurrentHandle);
    }
}

