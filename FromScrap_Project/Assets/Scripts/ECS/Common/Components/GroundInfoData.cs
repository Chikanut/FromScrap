using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct GroundInfoData : IComponentData
{
    public float CheckDistance;
    public CollisionFilter CollisionFilter;
    
    public float3 GroundPosition;
    public float3 GroundNormal;
    public bool isGrounded;

    public bool isLocalDown;
}
