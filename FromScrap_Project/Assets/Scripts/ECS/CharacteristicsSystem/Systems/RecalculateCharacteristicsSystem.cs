using BovineLabs.Event.Containers;
using BovineLabs.Event.Systems;
using ECS.SignalSystems.Systems;
using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace StatisticsSystem.Systems
{
    public partial class RecalculateCharacteristicsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private EventSystem _eventSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _eventSystem = World.GetOrCreateSystem<EventSystem>();
        }
        
        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            NativeEventStream.ThreadWriter installEvent = _eventSystem.CreateEventWriter<KitInstalledSignal>();
            
            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, ref CharacteristicsComponent statistics, ref DynamicBuffer<CharacteristicModificationsBuffer> modifications) =>
            {
                var prevStats = statistics.Value;
            
                statistics.Value = new Characteristics();
                
                var modificationsToRemove = new NativeList<int>(modifications .Length, Allocator.Temp);
                
                for (int i = 0; i < modifications .Length; i++)
                {
                    var modificator = modifications [i];

                    if (modificator.ModificatorHolder == Entity.Null)
                    {
                        modificationsToRemove.Add(i);
                        continue;
                    }

                    if(!modificator.Multiply)
                        statistics.Add(modificator.Value);
                    else
                        statistics.Multiply(modificator.Value);
                    
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
                    ecb.AddComponent(entityInQueryIndex, entity, new NewCharacteristicsTag());
                    installEvent.Write(new KitInstalledSignal() {Car = entity});
                }
                
            }).ScheduleParallel(Dependency);
            
            _eventSystem.AddJobHandleForProducer<KitInstalledSignal>(Dependency);
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}