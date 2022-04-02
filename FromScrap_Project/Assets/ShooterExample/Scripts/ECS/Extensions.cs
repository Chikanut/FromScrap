using System.ComponentModel;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public static class ECS_Math_Extensions
{
    public static float3 WorldToLocal(this float4x4 transform, float3 point)
    {
        return math.transform(math.inverse(transform), point);
    }

    public static float3 LocalToWorld(this float4x4 transform, float3 point)
    {
        return math.transform(transform, point);
    }

    public static float3 WorldToLocalDirection(this LocalToWorld localToWorld, float3 point)
    {
        return math.normalize(localToWorld.Value.LocalToWorld(point) - localToWorld.Position);
    }

    public static float Angle(this float3 dir1, float3 dir2)
    {
        var angle = math.acos(math.dot(math.normalize(dir1), math.normalize(dir2)));
        return math.degrees(angle);
    }
    
    public static float AngleSigned(this float3 from, float3 to, float3 axis)
    {
        float angle = math.acos(math.dot(math.normalize(from), math.normalize(to)));
        float sign = math.sign(math.dot(axis, math.cross(from, to)));
        float signedAngle = math.degrees(angle * sign);
        return float.IsNaN(signedAngle) ? 0 : signedAngle;
    }
    
    public static float Magnitude(this float3 vector) => (float) math.sqrt(vector.x * (double) vector.x + vector.y * (double) vector.y + vector.z * (double) vector.z);

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
    
    public static float SmoothDamp(
        float current,
        float target,
        ref float currentVelocity,
        float smoothTime,
        float maxSpeed)
    {
        float deltaTime = Time.deltaTime;
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }
    
    public static float SmoothDamp(
        float current,
        float target,
        ref float currentVelocity,
        float smoothTime)
    {
        float deltaTime = Time.deltaTime;
        float maxSpeed = float.PositiveInfinity;
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }

    public static float SmoothDamp(
        float current,
        float target,
        ref float currentVelocity,
        float smoothTime,
        [DefaultValue("Mathf.Infinity")] float maxSpeed,
        [DefaultValue("Time.deltaTime")] float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num1 = 2f / smoothTime;
        float num2 = num1 * deltaTime;
        float num3 = (float) (1.0 / (1.0 + (double) num2 + 0.479999989271164 * (double) num2 * (double) num2 + 0.234999999403954 * (double) num2 * (double) num2 * (double) num2));
        float num4 = current - target;
        float num5 = target;
        float max = maxSpeed * smoothTime;
        float num6 = Mathf.Clamp(num4, -max, max);
        target = current - num6;
        float num7 = (currentVelocity + num1 * num6) * deltaTime;
        currentVelocity = (currentVelocity - num1 * num7) * num3;
        float num8 = target + (num6 + num7) * num3;
        if ((double) num5 - (double) current > 0.0 == (double) num8 > (double) num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }

}