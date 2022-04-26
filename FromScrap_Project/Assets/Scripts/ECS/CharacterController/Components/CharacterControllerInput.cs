using Unity.Entities;
using Unity.Mathematics;

public struct CharacterControllerInput : IComponentData
{
    public float2 Movement;
    public float Rotation;
    public int Jumped;
}