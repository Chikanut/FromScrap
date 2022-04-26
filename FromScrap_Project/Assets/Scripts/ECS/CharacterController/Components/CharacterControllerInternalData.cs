using Unity.Entities;
using Unity.Mathematics;

public struct CharacterControllerInternalData : IComponentData
{
    public float CurrentRotationAngle;
    public CharacterControllerUtilities.CharacterSupportState SupportedState;
    public float3 UnsupportedVelocity;
    public float3 LinearVelocity;
    public Entity Entity;
    public bool IsJumping;
    public CharacterControllerInput Input;
}