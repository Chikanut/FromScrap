using LevelingSystem.Components;
using Unity.Entities;

namespace LevelingSystem.Systems
{
    public partial class ApplyExperienceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref DynamicBuffer<AddExperienceBuffer> addExperienceBuffer,
                ref LevelComponent levelComponent, ref DynamicBuffer<NewLevelBuffer> newLevelBuffer, in DynamicBuffer<LevelsInfoBuffer> levelsInfo) =>
            {
                for (int i = 0; i < addExperienceBuffer.Length; i++)
                {
                    levelComponent.Experience += addExperienceBuffer[i].Value;
                }
                
                addExperienceBuffer.Clear();

                if (levelsInfo.Length <= levelComponent.Level + 1) return;
                if (levelComponent.Experience < levelsInfo[levelComponent.Level].TargetExperience) return;
                
                newLevelBuffer.Add(new NewLevelBuffer() {Level = levelComponent.Level});
                        
                levelComponent.Experience = 0;
                levelComponent.Level++;
            }).ScheduleParallel();
        }
    }
}