using IsVisible.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace IsVisible.Systems
{
    public partial class IsVisibleSystem : SystemBase
    {
        private Camera _camera;
        private Plane _groundPlane;
        private Vector2 _screenSize;

        private static NativeArray<float3> _cameraPoints = new NativeArray<float3>(4, Allocator.Persistent);

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _camera = Camera.main;
            _groundPlane = new Plane(Vector3.up, Vector3.zero);
            _screenSize = new Vector2(Screen.width, Screen.height);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cameraPoints.Dispose();
        }

        protected override void OnUpdate()
        {
            PrepareCameraPoints();
            
            if(_camera == null)
                return;
            
            var groundViewArea = GetArea(_cameraPoints);
            var cameraPoints = _cameraPoints.AsReadOnly();
            
            Entities.ForEach((ref IsVisibleComponent isVisible, in LocalToWorld localToWorld) =>
            {
                var objectPos = localToWorld.Position;
                objectPos.y = 0;

                var closestDistance = float.MaxValue;
                var targetArea = 0f;

                targetArea += CalculateArea(objectPos, cameraPoints[0], cameraPoints[1], ref closestDistance);
                targetArea += CalculateArea(objectPos, cameraPoints[1], cameraPoints[2], ref closestDistance);
                targetArea += CalculateArea(objectPos, cameraPoints[2], cameraPoints[3], ref closestDistance);
                targetArea += CalculateArea(objectPos, cameraPoints[3], cameraPoints[0], ref closestDistance);
                
                isVisible.Value = !(targetArea - groundViewArea > 1 && closestDistance > isVisible.ObjectRadius);
                
            }).WithReadOnly(cameraPoints).ScheduleParallel();
        }

        static float CalculateArea(float3 targetPoint, float3 pointA, float3 pointB, ref float closestDistance)
        {
            var potentialClosestDistance = 0f;
            var triangle = new NativeArray<float3>(3, Allocator.Temp);
            
            triangle[0] = targetPoint;
            triangle[1] = pointA; 
            triangle[2] = pointB;
                
            potentialClosestDistance = FindDistanceToLine(pointA, pointB, targetPoint);
            
            if (potentialClosestDistance < closestDistance)
                closestDistance = potentialClosestDistance;
            
            var area = GetArea(triangle);

            triangle.Dispose();
            
            return area;
        }

        static float FindDistanceToLine(float3 origin, float3 end, float3 point)
        {
            var closestPoint = FindNearestPointOnLine(origin, end, point);

            return math.distance(point, closestPoint);
        }

        static float3 FindNearestPointOnLine(float3 origin, float3 end, float3 point)
        {
            var heading = (end - origin);
            var magnitudeMax = math.length(heading);
            heading = math.normalize(heading);
            
            var lhs = point - origin;
            var dotP = math.dot(lhs, heading);
            dotP = math.clamp(dotP, 0f, magnitudeMax);
            return origin + heading * dotP;
        }
        void PrepareCameraPoints()
        {
            _screenSize = new Vector2(Screen.width, Screen.height);
            
            _cameraPoints[0] = GetCameraPointOnPlane(0, 0);
            _cameraPoints[1] = GetCameraPointOnPlane(_screenSize.x, 0);
            _cameraPoints[2] = GetCameraPointOnPlane(_screenSize.x, _screenSize.y);
            _cameraPoints[3] = GetCameraPointOnPlane(0, _screenSize.y);
        }

        Vector3 GetCameraPointOnPlane(float X, float Y)
        {
            var spawnPoint = Vector3.zero;
            
            if(_camera == null)
                _camera = Camera.main;
            
            if(_camera == null)
                return spawnPoint;

            var ray = _camera.ScreenPointToRay(new Vector3(X, Y, 0));
            
            if (_groundPlane.Raycast(ray, out var enter))
                spawnPoint = ray.GetPoint(enter);

            return spawnPoint;
        }

        static float GetArea(NativeArray<float3> vertices)
        {
            if(vertices.Length < 3)
            {
                return 0;
            }

            var area = GetDeterminant(vertices[vertices.Length - 1].x, vertices[vertices.Length - 1].z, vertices[0].x,
                vertices[0].z);
            
            for (var i = 1; i < vertices.Length; i++)
            {
                area += GetDeterminant(vertices[i - 1].x, vertices[i - 1].z, vertices[i].x, vertices[i].z);
            }
            
            return math.abs(area / 2);
        }
        
        static float GetDeterminant(float x1, float y1, float x2, float y2)
        {
            return x1 * y2 - x2 * y1;
        }
    }
}