using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public static class ECS_Extensions
{
    public static float3 WorldToLocal(this float4x4 transform, float3 point)
    {
        return math.transform(math.inverse(transform), point);
    }
 
    public static float3 LocalToWorld(this float4x4 transform, float3 point)
    {
        return math.transform(transform, point);
    }
}
