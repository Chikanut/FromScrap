using Unity.Entities;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct RotateTowardsTarget : IComponentData
    {
        public float RotationSpeed;
        public float IsRotatedRadius;
        public bool IsRotated;
    }
}
