using SpawnObjectKit.Components;
using Unity.Entities;
using UnityEngine;


namespace SpawnObjectKit.Authorings
{
    public class SpawnObjectKitAuthoring : SpawnAfterDeathAuthoring
    {
        public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            base.ConvertAncestors(entity, dstManager, conversionSystem);
            dstManager.AddComponentData(entity, new SpawnObjectKitComponent());
        }
    }
}