using Unity.Entities;

[GenerateAuthoringComponent]
public struct RotateTowardsTarget : IComponentData
{
    public bool VerticalRotation;
    public float RotationSpeed;
    public float IsRotatedRadius;
    public bool IsRotated;
}

