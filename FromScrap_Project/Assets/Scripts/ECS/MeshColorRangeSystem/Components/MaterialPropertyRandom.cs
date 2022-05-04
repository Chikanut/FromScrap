using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Random", MaterialPropertyFormat.Float, -1)]
[GenerateAuthoringComponent]
public struct MaterialPropertyRandom : IComponentData
{
    public float Value;
}
