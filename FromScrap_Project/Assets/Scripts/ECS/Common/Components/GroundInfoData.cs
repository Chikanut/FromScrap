using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct GroundInfoData : IComponentData
{
    public float CheckDistance;
    public CollisionFilter CollisionFilter;

    public RaycastHit Info;
    public bool isGrounded;

    public bool isLocalDown;
}
