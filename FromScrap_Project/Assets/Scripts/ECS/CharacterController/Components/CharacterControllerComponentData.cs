using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CharacterControllerComponentData : IComponentData
{
    public float3 Gravity;
    public float MovementSpeed;
    public float MaxMovementSpeed;
    public float RotationSpeed;
    public float JumpUpwardsSpeed;
    public float MaxSlope; // radians
    public int MaxIterations;
    public float CharacterMass;
    public float SkinWidth;
    public float ContactTolerance;
    public byte AffectsPhysicsBodies;
    public byte RaiseCollisionEvents;
    public byte RaiseTriggerEvents;
}