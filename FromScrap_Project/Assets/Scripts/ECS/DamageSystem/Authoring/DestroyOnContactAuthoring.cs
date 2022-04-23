using DamageSystem.Components;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

public class DestroyOnContactAuthoring : SpawnAfterDeathAuthoring
{
    [Header("Collision info")]
    public bool IncludeTriggerEvents;
    public bool IncludeCollisionEvents;
    public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);
        
        dstManager.AddComponentData(entity, new DestroyOnContact(){IncludeTriggerEvent = IncludeTriggerEvents, IncludeCollisionEvents = IncludeCollisionEvents});
        
        if(IncludeTriggerEvents)
            dstManager.AddBuffer<StatefulTriggerEvent>(entity);
        if(IncludeCollisionEvents)
            dstManager.AddBuffer<StatefulCollisionEvent>(entity);
    }
}
