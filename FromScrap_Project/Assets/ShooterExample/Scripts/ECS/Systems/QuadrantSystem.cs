using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.InputSystem;

public struct QuadrantData : IComponentData {
    public Entity entity;
    public float3 position;
    public QuadrantEntity quadrantEntity;
}

public partial class QuadrantSystem : SystemBase {

    public static NativeMultiHashMap<int, QuadrantData> quadrantDataHashMap;
    
    public const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 25;

    public static int GetPositionHashMapKey(float3 position) {
        return (int) (math.floor(position.x / quadrantCellSize) + (quadrantYMultiplier * math.floor(position.y / quadrantCellSize)));
    }
    
    protected override void OnCreate() 
    {
        quadrantDataHashMap = new NativeMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
        base.OnCreate();
    }

    protected override void OnDestroy() {
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
        
        Entities.WithAll<QuadrantEntity, Translation>().WithChangeFilter<Translation>().ForEach((Entity entity, ref Translation translation, ref QuadrantEntity quadrantEntity) =>
        {
            var hashMapKey = GetPositionHashMapKey(translation.Value);
            quadrantEntity.HashKey = hashMapKey;
            nativeMultiHashMap.Add(hashMapKey, new QuadrantData { 
                entity = entity, 
                position = translation.Value ,
                quadrantEntity = quadrantEntity,
            });
        }).ScheduleParallel();
    }
}

