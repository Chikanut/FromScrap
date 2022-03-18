
using Unity.Entities;
using Unity.Mathematics;

public struct MovementComponent : IComponentData
{
    public float MovementSpeed;
    public float2 Limits;
}
