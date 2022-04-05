using Unity.Entities;

[GenerateAuthoringComponent]
public struct FindTargetData : IComponentData
{
    public float Range;
    public QuadrantEntityData.TypeNum TargetType;
}
