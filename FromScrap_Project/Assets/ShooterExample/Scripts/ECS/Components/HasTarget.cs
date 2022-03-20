using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct HasTarget : IComponentData
{
    public Entity TargetEntity;
    public float3 TargetPosition;
}
