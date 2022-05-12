using StatisticsSystem.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.StatisticsSystem.Authorings
{
    public class CharacteristicsModificationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool Multiply;
        public Characteristics Value;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity,
                new CharacteristicsModificationComponent()
                    {Multiply = Multiply, Value = Value, CurrentTryUpdateTimes = 0});
        }
    }
}