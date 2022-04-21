using Unity.Entities;
using Unity.Mathematics;

public struct TrailEffectLastInfoData : IBufferElementData
{
    public float3 Point_Center;
    public float3 Point1_Lt;
    public float3 Point2_Rt;
    public float2 UVPos1_Lt;
    public float2 UVPos2_Rt;
    public float Lifetime;
    public bool IsActive;
}
