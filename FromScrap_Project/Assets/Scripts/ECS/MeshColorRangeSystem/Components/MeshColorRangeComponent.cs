using Unity.Entities;
using Unity.Mathematics;

public struct MeshColorRangeComponent : IComponentData
{
   public float HueBaseValue;
   public float HueRange;
   public float SaturationBaseValue;
   public float SaturationRange;
   public float2 RandomRange;
}