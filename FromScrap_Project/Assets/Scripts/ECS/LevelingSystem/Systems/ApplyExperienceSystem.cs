using BovineLabs.Event.Systems;
using LevelingSystem.Components;
using Unity.Entities;

namespace LevelingSystem.Systems
{
    [UpdateAfter(typeof(GatherExperienceSystem))]
    public partial class ApplyExperienceSystem : SystemBase
    {
        private EventSystem _eventSystem;
        protected override void OnCreate()
        {
            _eventSystem = this.World.GetOrCreateSystem<EventSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var experienceEvent = _eventSystem.CreateEventWriter<OnExperienceChangeSignal>();
            var levelUpEvent = _eventSystem.CreateEventWriter<LevelUpSignal>();
            
            Entities.ForEach((ref DynamicBuffer<AddExperienceBuffer> addExperienceBuffer,
                ref LevelComponent levelComponent, ref DynamicBuffer<NewLevelBuffer> newLevelBuffer, in DynamicBuffer<LevelsInfoBuffer> levelsInfo) =>
            {
                for (int i = 0; i < addExperienceBuffer.Length; i++)
                {
                    levelComponent.Experience += addExperienceBuffer[i].Value;
  
                    experienceEvent.Write(new OnExperienceChangeSignal()
                    {
                        Experience = levelComponent.Experience
                    });
                }
                
                addExperienceBuffer.Clear();
                
                while (levelsInfo.Length > levelComponent.Level + 1 && levelComponent.Experience >= levelsInfo[levelComponent.Level].TargetExperience)
                {
                    newLevelBuffer.Add(new NewLevelBuffer() {Level = levelComponent.Level});

                    levelUpEvent.Write(new LevelUpSignal()
                    {
                        Level = levelComponent.Level
                    });
                    
                    levelComponent.Experience -= levelsInfo[levelComponent.Level].TargetExperience;
                    levelComponent.Level++;
                    
                }
            }).ScheduleParallel();
            
            _eventSystem.AddJobHandleForProducer<OnExperienceChangeSignal>(Dependency);
        }
    }
}