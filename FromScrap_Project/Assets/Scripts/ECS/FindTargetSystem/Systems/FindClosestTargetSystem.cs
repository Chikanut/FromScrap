using ECS.Common;
using IsVisible.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.FindTargetSystem
{
    public partial class FindClosestTargetSystem : SystemBase
    {
        private EntityQuery _targetsQuery;
        private EntityQuery _searchingQuery;
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
        
            _targetsQuery = GetEntityQuery(
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<IsVisibleComponent>(),
                ComponentType.ReadOnly<ObjectTypeComponent>()
            );
        
            _searchingQuery = GetEntityQuery(
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<FindTargetData>(),
                ComponentType.ReadOnly<TargetSearchRadiusComponent>()
            );
        
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }


        protected override void OnUpdate()
        {
            // if (_targetsQuery.CalculateEntityCount() == 0) return;
            
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            
            Dependency = FindTarget(EntityObjectType.Player, ecb, Dependency);
            Dependency = FindTarget(EntityObjectType.Unit, ecb, Dependency);
            Dependency = FindTarget(EntityObjectType.Object, ecb, Dependency);
            Dependency = FindTarget(EntityObjectType.Collectable, ecb, Dependency);
            Dependency = FindTarget(EntityObjectType.Projectile, ecb, Dependency);
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        public JobHandle FindTarget(EntityObjectType type, EntityCommandBuffer.ParallelWriter ecb, JobHandle dependency)
        {
            if(_targetsQuery.HasFilter())
                _targetsQuery.ResetFilter();
            
            _targetsQuery.SetSharedComponentFilter(new ObjectTypeComponent {Type = type});
            
            if(_searchingQuery.HasFilter())
                _searchingQuery.ResetFilter();
            
            _searchingQuery.SetSharedComponentFilter(new FindTargetData {TargetType = type});

            if (_targetsQuery.CalculateEntityCount() == 0) return dependency;
            
            var targets = _targetsQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
            var entities = _targetsQuery.ToEntityArray(Allocator.TempJob);
            var hasTargetFilter = GetComponentDataFromEntity<HasTarget>(true);
            
            var handle = new FindTargetJob()
            {
                SearchingObjectType = EntityObjectType.Player, Ecb = ecb, Targets = targets, Entities = entities,
                HasTargetFilter = hasTargetFilter
            }.ScheduleParallel(_searchingQuery);

            targets.Dispose(handle);
            entities.Dispose(handle);

            return JobHandle.CombineDependencies(Dependency, handle);
        }

        [BurstCompile]
        public partial struct FindTargetJob : IJobEntity
        {
            [ReadOnly] public EntityObjectType SearchingObjectType;
            [ReadOnly] public NativeArray<LocalToWorld> Targets;
            [ReadOnly] public NativeArray<Entity> Entities;
            [ReadOnly] public ComponentDataFromEntity<HasTarget> HasTargetFilter;

            public EntityCommandBuffer.ParallelWriter Ecb;

            void Execute(Entity entity, [EntityInQueryIndex] int entityInQueryIndex, in LocalToWorld translation,
                in TargetSearchRadiusComponent targetSearchRadiusComponent)
            {
                var unitPosition = translation.Position;
                var targetEntity = Entity.Null;
                var targetPosition = new float3(0, 0, 0);
                var targetDistance = float.MaxValue;

                for (int i = 0; i < Targets.Length; i++)
                {
                    var dist = math.distance(unitPosition, Targets[i].Position);

                    if (dist > targetSearchRadiusComponent.Radius) continue;
                    if (dist > targetDistance) continue;

                    targetDistance = dist;
                    targetPosition = Targets[i].Position;
                    targetEntity = Entities[i];
                }

                if (targetEntity == Entity.Null && HasTargetFilter.HasComponent(entity))
                {
                    Ecb.RemoveComponent<HasTarget>(entityInQueryIndex, entity);
                }
                else if (targetEntity != Entity.Null && !HasTargetFilter.HasComponent(entity))
                {
                    Ecb.AddComponent(entityInQueryIndex, entity,
                        new HasTarget {TargetEntity = targetEntity, TargetPosition = targetPosition});
                }
                else if (targetEntity != Entity.Null && HasTargetFilter.HasComponent(entity))
                {
                    Ecb.SetComponent(entityInQueryIndex, entity,
                        new HasTarget {TargetEntity = targetEntity, TargetPosition = targetPosition});
                }


            }
        }
    }
}