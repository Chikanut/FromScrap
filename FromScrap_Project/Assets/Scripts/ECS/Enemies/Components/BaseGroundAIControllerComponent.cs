using Unity.Entities;

[GenerateAuthoringComponent]
public struct BaseGroundAIControllerComponent : IComponentData
{
    public float MaxMovementDir;
}
