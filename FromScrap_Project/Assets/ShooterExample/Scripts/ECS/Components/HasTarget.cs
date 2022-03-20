using Unity.Entities;
using Unity.Mathematics;

public struct HasTarget : IComponentData
{
    public Entity TargetEntity;
    public float3 TargetPosition;
}
