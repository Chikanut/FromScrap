using Cars.View.Components;
using LevelingSystem.Components;
using Packages.Common.Storage.Config.Cars;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace ECS.LevelingSystem.Authoring
{
    public class LevelingSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        private ICarsConfigController _carsConfigController;
        
        [Inject]
        public void Init(ICarsConfigController carsConfigController)
        {
            _carsConfigController = carsConfigController;
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            ProjectContext.Instance.Container.Inject(this);
            
            dstManager.AddComponentData(entity, new LevelComponent() {Experience = 0, Level = 0});
            dstManager.AddBuffer<AddExperienceBuffer>(entity);
            dstManager.AddBuffer<NewLevelBuffer>(entity);
            
            var id = dstManager.GetComponentData<CarIDComponent>(entity).ID;
            var carLevelsInfo = _carsConfigController.GetCarData(id).LevelsExperience;
            var infoBuffer = dstManager.AddBuffer<LevelsInfoBuffer>(entity);
            foreach (var levelInfo in carLevelsInfo)
            {
                infoBuffer.Add(new LevelsInfoBuffer() {TargetExperience = levelInfo});
            }
        }
    }
}