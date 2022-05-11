using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Collections;
using Unity.Entities;

namespace StatisticsSystem.Systems
{
    public partial class RecalculateStatisticsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, ref StatisticsComponent statistics, ref DynamicBuffer<StatisticModificationsBuffer> modifications) =>
            {
                var prevStats = statistics.Value;
            
                statistics.Value = new Statistics();
                
                var modificationsToRemove = new NativeList<int>(modifications .Length, Allocator.Temp);
                
                for (int i = 0; i < modifications .Length; i++)
                {
                    var modificator = modifications [i];

                    if (modificator.ModificatorHolder == Entity.Null)
                    {
                        modificationsToRemove.Add(i);
                        continue;
                    }

                    statistics.Add(modificator.Value);
                    modificationsToRemove.Add(-1);
                }

                for (int i = 0; i < modificationsToRemove.Length; i++)
                {
                    if (modificationsToRemove[i] == -1)
                        continue;
                    
                    modifications.RemoveAtSwapBack(modificationsToRemove[i]);
                }

                if (statistics.Value.CompareTo(prevStats) != 0)
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new StatisticsUpdatedTag());
                }
                
            }).ScheduleParallel(Dependency);
            
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}