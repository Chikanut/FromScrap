using Unity.Entities;

[GenerateAuthoringComponent]
public struct FindTargetData : IComponentData
{
    public float Range;
    public QuadrantEntity.TypeNum TargetType;
}
