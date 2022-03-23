using Unity.Entities;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct ShotData : IComponentData
    {
        public float Velocity;
        public float Lifetime;
    }
}