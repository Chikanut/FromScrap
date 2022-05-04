using Unity.Entities;
using Unity.Rendering;

[MaterialProperty("_Saturation", MaterialPropertyFormat.Float, -1)]
[GenerateAuthoringComponent]
public struct MaterialPropertySaturation : IComponentData
{
    public float Value;
}