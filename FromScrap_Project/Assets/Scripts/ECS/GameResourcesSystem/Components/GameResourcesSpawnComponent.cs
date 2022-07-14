using Unity.Entities;

namespace ECS.GameResourcesSystem.Components
{
    public struct GameResourcesSpawnComponent :  IComponentData
    {
        public Entity DynamicTerrainEntity;
        public bool IsLoaded;
    }
}
