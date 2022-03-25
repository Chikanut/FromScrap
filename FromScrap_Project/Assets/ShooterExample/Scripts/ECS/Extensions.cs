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

    public static float Angle(this float3 dir1, float3 dir2)
    {
        var angle = math.acos(math.dot(math.normalize(dir1), math.normalize(dir2)));
        return math.degrees(angle);
    }
    
    public static float3 RotateAroundPoint(this float3 position, float3 pivot, float3 axis, float delta) => math.mul(quaternion.AxisAngle(axis, delta), position - pivot) + pivot;
    
    public static void SetTranslationToWorldPosition(this ref Translation translation, [ReadOnly] LocalToWorld localToWorld, [ReadOnly] float3 worldPosition)
    {
        translation.Value += math.mul(math.inverse(localToWorld.Value), new float4(worldPosition, 1)).xyz;
    }
    
    public static float3 WorldPositionToLocal(this ref float3 worldPosition, [ReadOnly] Translation localPosition, [ReadOnly] LocalToWorld localToWorld)
    {
        return localPosition.Value + math.mul(math.inverse(localToWorld.Value), new float4(worldPosition, 1)).xyz;
    }
}
