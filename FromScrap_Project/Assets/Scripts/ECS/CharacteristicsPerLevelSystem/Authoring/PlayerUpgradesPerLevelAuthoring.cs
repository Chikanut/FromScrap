using CharacteristicsPerLevelSystem.Components;
using StatisticsSystem.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CharacteristicsPerLevelSystem.Authoring
{
    public class PlayerUpgradesPerLevelAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (!dstManager.HasComponent<CharacteristicModificationsBuffer>(entity))
            {
                Debug.LogError("GameObject " + gameObject.name + " has no CharacteristicModificationsBuffer component");
                return;
            }

            var child = dstManager.CreateEntity();
            dstManager.SetName(child, "PlayerUpgradesPerLevel");
            dstManager.AddComponentData(child, new Parent() {Value = entity});
            dstManager.AddComponentData(entity, new LocalToParent());
            
            dstManager.AddBuffer<UpgradesPerLevelBuffer>(entity);
            dstManager.AddComponentData(entity, new UpgradePerLevelListener()
            {
                Target = child
            });
        }
    }
}