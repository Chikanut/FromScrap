using Unity.Entities;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct RotateTowardsTarget : IComponentData
    {
        public bool VerticalRotation;
        public float RotationSpeed;
        public float IsRotatedRadius;
        public bool IsRotated;
    }
}
