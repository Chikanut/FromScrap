using MyBox;
using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using UnityEngine;

namespace ECS.StatisticsSystem.Authorings
{
    public class StatisticsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public enum Type
        {
            Parent, 
            Child
        }
        
        public Type StatisticsType;
        [ConditionalField(nameof(StatisticsType), true, Type.Parent)]public Statistics DefaultStatistics;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new StatisticsComponent(){Statistics = DefaultStatistics});
            if (StatisticsType == Type.Parent)
            {
                dstManager.AddBuffer<StatisticModificationsBuffer>(entity).Add(new StatisticModificationsBuffer()
                    {Modificator = DefaultStatistics, ModificatorHolder = entity});
                dstManager.AddComponentData(entity, new StatisticsUpdatedTag());
            }
            else
            {
                dstManager.AddComponentData(entity, new GetStatisticTag());
            }
        }
    }
}