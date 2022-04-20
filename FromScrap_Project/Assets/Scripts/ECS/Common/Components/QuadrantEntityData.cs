using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct QuadrantEntityData : IComponentData
{
    public TypeNum Type;
    public enum TypeNum
    {
        Unit,
        Object,
        Collectable,
        Player
    }
    
    public int HashKey;
}
