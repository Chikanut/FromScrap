﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FindClosestTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var quadrantDataHashMap = QuadrantSystem.QuadrantDataHashMap;
        Entities.WithAll<FindTargetData, QuadrantEntity, HasTarget>().ForEach((Entity entity, ref HasTarget target,in Translation translation,
            in FindTargetData findTargetData, in QuadrantEntity quadrantEntity) =>
        {

            var unitHashMapKey = quadrantEntity.HashKey;
            var unitPosition = translation.Value;
            var targetEntity = Entity.Null;
            var targetPosition = new float3(0, 0, 0);

            TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey, unitPosition, findTargetData.TargetType,
                ref targetEntity, ref targetPosition);
            
            if (targetEntity == Entity.Null)
            {
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey - 1, unitPosition, findTargetData.TargetType,
                    ref targetEntity, ref targetPosition); // Left
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey + 1, unitPosition, findTargetData.TargetType,
                    ref targetEntity, ref targetPosition); // Right
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey + QuadrantSystem.quadrantYMultiplier - 1,
                    unitPosition, findTargetData.TargetType, ref targetEntity, ref targetPosition); // Up Left
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey + QuadrantSystem.quadrantYMultiplier,
                    unitPosition, findTargetData.TargetType, ref targetEntity, ref targetPosition); // Up Center
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey + QuadrantSystem.quadrantYMultiplier + 1,
                    unitPosition, findTargetData.TargetType, ref targetEntity, ref targetPosition); // Up Right
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey - QuadrantSystem.quadrantYMultiplier - 1,
                    unitPosition, findTargetData.TargetType, ref targetEntity, ref targetPosition); // Down Left
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey - QuadrantSystem.quadrantYMultiplier,
                    unitPosition, findTargetData.TargetType, ref targetEntity, ref targetPosition); // Down Center
                TrySetClosestTarget(quadrantDataHashMap, unitHashMapKey - QuadrantSystem.quadrantYMultiplier + 1,
                    unitPosition, findTargetData.TargetType, ref targetEntity, ref targetPosition); // Down Right
            }

            if (math.distance(unitPosition, targetPosition) > findTargetData.Range) {
                targetEntity = Entity.Null;
            }

            target.TargetEntity = targetEntity;
            target.TargetPosition = targetPosition;

        }).WithReadOnly(quadrantDataHashMap).WithDisposeOnCompletion(quadrantDataHashMap).ScheduleParallel();
    }
    
    private static void TrySetClosestTarget(NativeMultiHashMap<int, QuadrantData> targetHashMap, int quadrantHashMapKey, float3 unitPosition, QuadrantEntity.TypeNum unitTypeEnum, ref Entity targetEntity, ref float3 targetPosition)
    {
        if (!targetHashMap.TryGetFirstValue(quadrantHashMapKey, out var targetQuadrantData,
                out var nativeMultiHashMapIterator)) return;
        do
        {
            if (targetQuadrantData.quadrantEntity.Type != unitTypeEnum) continue;
            
            if (targetEntity == Entity.Null) {
                targetEntity = targetQuadrantData.entity;
                targetPosition = targetQuadrantData.position;
            } else {
                // Has target, closest?
                // ######## TODO: REPLACE WITH math.select();
                if (math.distance(unitPosition, targetQuadrantData.position) < math.distance(unitPosition, targetPosition)) {
                    // New Target closer
                    targetEntity = targetQuadrantData.entity;
                    targetPosition = targetQuadrantData.position;
                }
            }
        } while (targetHashMap.TryGetNextValue(out targetQuadrantData, ref nativeMultiHashMapIterator));
    }
}