using DamageSystem.Components;
using Unity.Entities;
using UnityEngine;

public class DestroyOnContactAuthoring : SpawnAfterDeathAuthoring
{
    [Header("Collision info")]
    public bool IncludeTriggerEvents;
    public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);
        
        dstManager.AddComponentData(entity, new DestroyOnContact(){IncludeTriggerEvent = IncludeTriggerEvents});
    }
}
