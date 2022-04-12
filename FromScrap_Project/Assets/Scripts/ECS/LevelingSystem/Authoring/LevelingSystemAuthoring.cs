using System.Collections.Generic;
using LevelingSystem.Components;
using Unity.Entities;
using UnityEngine;

namespace ECS.LevelingSystem.Authoring
{
    public class LevelingSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public List<int> LevelsInfo = new List<int>();

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new LevelComponent() {Experience = 0, Level = 0});
            
            dstManager.AddBuffer<AddExperienceBuffer>(entity);
            dstManager.AddBuffer<NewLevelBuffer>(entity);
            
            var infoBuffer = dstManager.AddBuffer<LevelsInfoBuffer>(entity);
            for (int i = 0; i < LevelsInfo.Count; i++)
            {
                infoBuffer.Add(new LevelsInfoBuffer() {TargetExperience = LevelsInfo[i]});
            }
        }
    }
}