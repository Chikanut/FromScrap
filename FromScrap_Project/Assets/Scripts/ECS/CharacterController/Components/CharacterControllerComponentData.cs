using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CharacterControllerComponentData : IComponentData
{
    public float MaxSpeed;
    public float Acceleration;
    public float MaxAcceleration;
    public float MaxSidewaysImpulse;
    public float RotationSpeed;
    public float LevelingPower;
}