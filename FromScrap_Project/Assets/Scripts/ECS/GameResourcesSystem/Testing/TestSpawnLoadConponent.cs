using Unity.Entities;

namespace ECS.LevelSpawnerSystem
{
    public struct TestSpawnLoadComponent :  IComponentData
    {
        public Entity SpawnEntity;
    }
}