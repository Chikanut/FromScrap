using Serializable.ResourcesManager;
using Unity.Entities;

namespace ECS.GameResourcesSystem.Components
{
    public struct GameResourcesSpawnEntityId :  IComponentData
    {
        public GameResourcesEntityTypeId TypeId;
    }
}