using System.ComponentModel;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


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

    public static float3 RotateAroundPoint(this float3 position, float3 pivot, float3 axis, float delta) =>
        math.mul(quaternion.AxisAngle(axis, delta), position - pivot) + pivot;

    public static void SetTranslationToWorldPosition(this ref Translation translation,
        [Unity.Collections.ReadOnly] LocalToWorld localToWorld, [Unity.Collections.ReadOnly] float3 worldPosition)
    {
        translation.Value += math.mul(math.inverse(localToWorld.Value), new float4(worldPosition, 1)).xyz;
    }

    public static void WorldPositionToLocal(this ref float3 worldPosition,
        [Unity.Collections.ReadOnly] Translation localPosition,
        [Unity.Collections.ReadOnly] LocalToWorld localToWorld)
    {
        worldPosition = localPosition.Value +
                        math.mul(math.inverse(localToWorld.Value), new float4(worldPosition, 1)).xyz;
    }

    public static float3 SmoothDamp(
        float3 current,
        float3 target,
        ref float3 currentVelocity,
        float smoothTime)
    {
        var deltaTime = Time.deltaTime;
        var maxSpeed = float.PositiveInfinity;
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }
    
    public static float3 SmoothDamp(
        float3 current,
        float3 target,
        ref float3 currentVelocity,
        float smoothTime,
        [DefaultValue("Mathf.Infinity")] float maxSpeed,
        [DefaultValue("Time.deltaTime")] float deltaTime)
    {
        smoothTime = math.max(0.0001f, smoothTime);
        float num1 = 2f / smoothTime;
        float num2 = num1 * deltaTime;
        float num3 = (float) (1.0 / (1.0 + (double) num2 + 0.479999989271164 * (double) num2 * (double) num2 +
                                     0.234999999403954 * (double) num2 * (double) num2 * (double) num2));
        float num4 = current.x - target.x;
        float num5 = current.y - target.y;
        float num6 = current.z - target.z;
        float3 vector3 = target;
        float num7 = maxSpeed * smoothTime;
        float num8 = num7 * num7;
        float d = (float) ((double) num4 * (double) num4 + (double) num5 * (double) num5 +
                           (double) num6 * (double) num6);
        if ((double) d > (double) num8)
        {
            float num9 = (float) math.sqrt((double) d);
            num4 = num4 / num9 * num7;
            num5 = num5 / num9 * num7;
            num6 = num6 / num9 * num7;
        }

        target.x = current.x - num4;
        target.y = current.y - num5;
        target.z = current.z - num6;
        float num10 = (currentVelocity.x + num1 * num4) * deltaTime;
        float num11 = (currentVelocity.y + num1 * num5) * deltaTime;
        float num12 = (currentVelocity.z + num1 * num6) * deltaTime;
        currentVelocity.x = (currentVelocity.x - num1 * num10) * num3;
        currentVelocity.y = (currentVelocity.y - num1 * num11) * num3;
        currentVelocity.z = (currentVelocity.z - num1 * num12) * num3;
        float x = target.x + (num4 + num10) * num3;
        float y = target.y + (num5 + num11) * num3;
        float z = target.z + (num6 + num12) * num3;
        float num13 = vector3.x - current.x;
        float num14 = vector3.y - current.y;
        float num15 = vector3.z - current.z;
        float num16 = x - vector3.x;
        float num17 = y - vector3.y;
        float num18 = z - vector3.z;
        if ((double) num13 * (double) num16 + (double) num14 * (double) num17 + (double) num15 * (double) num18 >
            0.0)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
            currentVelocity.x = (x - vector3.x) / deltaTime;
            currentVelocity.y = (y - vector3.y) / deltaTime;
            currentVelocity.z = (z - vector3.z) / deltaTime;
        }

        return new float3(x, y, z);
    }

}




// namespace Unity.Mathematics
// {
//     [System.Serializable]
//     [Il2CppEagerStaticClassConstruction]
//     public partial struct float3
//     {
     
//     }
// }