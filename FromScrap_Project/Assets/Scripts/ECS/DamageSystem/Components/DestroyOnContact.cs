using Unity.Entities;

namespace DamageSystem.Components
{
    public struct DestroyOnContact : IComponentData
    {
        public bool IncludeTriggerEvent;
        public bool IncludeCollisionEvents;
    }
}
