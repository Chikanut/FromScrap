using DamageSystem.Components;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

namespace DamageSystem.Authoring
{
    public class AddHealthAuthoring : SpawnAfterDeathAuthoring
    {
        [Header("Health")]
        public int HealthPoints;

        public override void ConvertAncestors(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            base.ConvertAncestors(entity, dstManager, conversionSystem);

            dstManager.AddComponentData(entity, new AddHealthComponent() {Value = HealthPoints});
            dstManager.AddBuffer<StatefulTriggerEvent>(entity);
        }
    }
}