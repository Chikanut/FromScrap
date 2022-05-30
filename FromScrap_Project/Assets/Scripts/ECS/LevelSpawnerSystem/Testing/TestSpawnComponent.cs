using Unity.Entities;

namespace ECS.LevelSpawnerSystem
{
    public struct TestSpawnComponent :  IComponentData
    {
        public Entity SpawnEntity;
        public int EntityCount;
        public bool EnableLoad;
        public bool EnableSpawn;
    }
}
