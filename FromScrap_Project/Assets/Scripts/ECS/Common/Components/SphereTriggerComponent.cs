using Unity.Entities;
using Unity.Physics;

public struct SphereTriggerComponent : IComponentData
{
    public float Radius;
    public float PrevRadius;
}
