using LevelingSystem.Components;
using Unity.Entities;
using UnityEngine;

public class ExperiencePointAuthoring : SpawnAfterDeathAuthoring
{
    [Header("Experience")]
    public int ExperiencePoints;

    public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);
        
        dstManager.AddComponentData(entity, new ExperienceComponent() {Value = ExperiencePoints});
    }
}
