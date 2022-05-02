using Unity.Entities;
using Unity.Mathematics;

public struct CharacterControllerInput : IComponentData
{
    public float3 Movement;
    public float Rotation;
}