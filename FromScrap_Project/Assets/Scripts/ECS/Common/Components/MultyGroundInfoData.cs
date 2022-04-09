using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct MultyGroundInfoData : IBufferElementData
{
    public CollisionFilter CollisionFilter;
    public float CheckDistance;
    public float3 AnchorPoints;
    
    public float3 GroundPosition;
    public float3 GroundNormal;
    public bool isGrounded;
    
    public bool isLocalDown;
}