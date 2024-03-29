﻿using MyBox;
using StatisticsSystem.Components;
using StatisticsSystem.Tags;
using Unity.Entities;
using UnityEngine;

namespace ECS.StatisticsSystem.Authorings
{
    public class CharacteristicsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public enum Type
        {
            Parent, 
            Child
        }
        
        public Type StatisticsType;
        [ConditionalField(nameof(StatisticsType), true, Type.Parent)] public bool AddDefaultCharacteristics;
        [ConditionalField(nameof(StatisticsType), true, Type.Parent)] public Characteristics defaultCharacteristics;
        [ConditionalField(nameof(StatisticsType), false, Type.Child)] public bool isLocalStatistics;
        [ConditionalField(nameof(StatisticsType), false, Type.Child)] public Characteristics localCharacteristics;
        
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CharacteristicsComponent(){Value = defaultCharacteristics});
            if (StatisticsType == Type.Parent)
            {
                var modifications = dstManager.AddBuffer<CharacteristicModificationsBuffer>(entity);
                
                if(AddDefaultCharacteristics)
                    modifications.Add(new CharacteristicModificationsBuffer {Value = defaultCharacteristics, ModificatorHolder = entity});
                
                dstManager.AddComponentData(entity, new NewCharacteristicsTag());
            }
            else
            {
                dstManager.AddComponentData(entity, new GetCharacteristicsTag());
                
                if (isLocalStatistics)
                {
                    dstManager.AddComponentData(entity, new LocalCharacteristicsComponent(){Value = localCharacteristics});
                }
            }
        }
    }
}