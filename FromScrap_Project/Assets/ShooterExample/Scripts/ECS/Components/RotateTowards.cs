using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RotateTowards : IComponentData
{
    public float3 Target;
    public float RotationSpeed;
}
