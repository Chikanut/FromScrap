using Unity.Entities;

namespace Collectables.Components
{
    public struct CollectableGatheredComponent : IComponentData
    {
        public Entity CollectedEntity;
    }
}