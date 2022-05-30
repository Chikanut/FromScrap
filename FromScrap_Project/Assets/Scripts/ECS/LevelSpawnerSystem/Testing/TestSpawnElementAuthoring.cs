using Unity.Entities;
using UnityEngine;

namespace ECS.LevelSpawnerSystem
{
    public class TestSpawnElementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TestSpawnElementId() {});
        }
    }
}
