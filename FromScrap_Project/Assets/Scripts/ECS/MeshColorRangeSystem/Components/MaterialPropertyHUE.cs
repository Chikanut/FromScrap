using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Hue", MaterialPropertyFormat.Float, -1)]
[GenerateAuthoringComponent]
public struct MaterialPropertyHUE : IComponentData
{
    public float Value;
}
