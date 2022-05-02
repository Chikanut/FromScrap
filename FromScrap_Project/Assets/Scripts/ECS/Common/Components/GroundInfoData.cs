using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

public struct GroundInfoData : IComponentData
{
    public float CheckDistance;
    public float3 CheckOffset;
    public bool isLocalDown;
    
    public CollisionFilter CollisionFilter;

    public RaycastHit Info;
    public bool isGrounded;



}
