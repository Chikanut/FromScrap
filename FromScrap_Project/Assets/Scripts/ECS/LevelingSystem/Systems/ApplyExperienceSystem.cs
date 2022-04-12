using LevelingSystem.Components;
using Unity.Entities;

namespace LevelingSystem.Systems
{
    [UpdateAfter(typeof(GatherExperienceSystem))]
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
                
                while (levelsInfo.Length > levelComponent.Level + 1 && levelComponent.Experience >= levelsInfo[levelComponent.Level].TargetExperience)
                {
                    newLevelBuffer.Add(new NewLevelBuffer() {Level = levelComponent.Level});

                    levelComponent.Experience -= levelsInfo[levelComponent.Level].TargetExperience;
                    levelComponent.Level++;
                }
            }).ScheduleParallel();
        }
    }
}