using Serializable.ResourcesManager;
using Unity.Entities;

namespace ECS.GameResourcesSystem.Components
{
    public struct GameResourcesLoadEntityId :  IComponentData
    {
        public Entity TargetEntity;
        public GameResourcesEntityTypeId TypeId;
    }
}