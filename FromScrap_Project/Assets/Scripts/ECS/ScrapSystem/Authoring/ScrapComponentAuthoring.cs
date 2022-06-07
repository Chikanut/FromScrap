using Scrap.Components;
using Unity.Entities;
using UnityEngine;

public class ScrapComponentAuthoring : SpawnAfterDeathAuthoring
{
    [Header("Scrap")]
    public int ScrapCount;

    public override void ConvertAncestors(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new ScrapComponent() {Value = ScrapCount});
    }
}
