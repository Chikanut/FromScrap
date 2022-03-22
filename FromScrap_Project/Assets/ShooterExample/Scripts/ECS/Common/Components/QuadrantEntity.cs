using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct QuadrantEntity : IComponentData
{
    public TypeNum Type;
    public enum TypeNum
    {
        Unit,
        Object,
        Player
    }
    
    public int HashKey;
}
