using DamageSystem.Components;
using Unity.Entities;

public class DestroyOnContactAuthoring : SpawnAfterDeathAuthoring
{
    public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);
        
        dstManager.AddComponentData(entity, new DestroyOnContact());
    }
}
