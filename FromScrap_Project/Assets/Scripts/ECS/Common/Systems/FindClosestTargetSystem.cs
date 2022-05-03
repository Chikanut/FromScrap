using IsVisible.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public partial class FindClosestTargetSystem : SystemBase
{
    private EntityQuery _targetsQuery;
    private EndSimulationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _targetsQuery = GetEntityQuery(
        ComponentType.ReadOnly<LocalToWorld>(),
            ComponentType.ReadOnly<IsVisibleComponent>(),
            ComponentType.ReadOnly<QuadrantEntityData>()
        );
        
        _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        if (_targetsQuery.CalculateEntityCount() == 0) return;
        
        var targets = _targetsQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var quadrantEntities = _targetsQuery.ToComponentDataArray<QuadrantEntityData>(Allocator.TempJob);
        var entities = _targetsQuery.ToEntityArray(Allocator.TempJob);
        
        var hasTargetFilter = GetComponentDataFromEntity<HasTarget>(true);
        
        var ecb = _ecbSystem.CreateCommandBuffer();

        Dependency = Entities.ForEach((Entity entity,
                in LocalToWorld translation,
                in FindTargetData findTargetData) =>
            {
                var unitPosition = translation.Position;
                var targetEntity = Entity.Null;
                var targetPosition = new float3(0, 0, 0);
                var targetDistance = float.MaxValue;

                for (int i = 0; i < targets.Length; i++)
                {
                    var dist = math.distance(unitPosition, targets[i].Position);

                    if (quadrantEntities[i].Type != findTargetData.TargetType) continue;
                    if (dist > findTargetData.Range) continue;
                    if (dist > targetDistance) continue;

                    targetDistance = dist;
                    targetPosition = targets[i].Position;
                    targetEntity = entities[i];
                }

                if (targetEntity == Entity.Null && hasTargetFilter.HasComponent(entity))
                {
                    ecb.RemoveComponent<HasTarget>(entity);
                }
                else if (targetEntity != Entity.Null && !hasTargetFilter.HasComponent(entity))
                {
                    ecb.AddComponent(entity,
                        new HasTarget() {TargetEntity = targetEntity, TargetPosition = targetPosition});
                }
                else if (targetEntity != Entity.Null && hasTargetFilter.HasComponent(entity))
                {
                    ecb.SetComponent(entity,
                        new HasTarget() {TargetEntity = targetEntity, TargetPosition = targetPosition});
                }
            }).WithReadOnly(hasTargetFilter)
            .WithReadOnly(entities)
            .WithReadOnly(targets)
            .WithReadOnly(quadrantEntities)
            .WithDisposeOnCompletion(entities)
            .WithDisposeOnCompletion(targets)
            .WithDisposeOnCompletion(quadrantEntities)
            .Schedule(Dependency);
        
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}