using Unity.Entities;

namespace ECS.LevelSpawnerSystem
{
    public struct TestSpawnElementId :  IComponentData
    {
        public Entity TargetEntity;
    }
}