﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace ArtResources.TrailEffectArt.Scripts
{
    public class TrailEffectVisualizationController : MonoBehaviour
    {
        public float PointsDistance = 0.5f;
        public float HeightOffset = 0.1f;
        public float TrackWidthVariation = 0.2f;
        public int MaxTrailPoints = 300;
        public float UVInc = 0.5f;
        public float trailLifetime = 5f;
        public bool fadeTrail = true;
        public int startFadePoints = 5;

        private readonly List<TrailPoint> _points = new List<TrailPoint>();
        private TrailPoint LastPoint;
        private float UVPos;
        private Mesh _trailMesh;

        public AnimationCurve ballSizeToTrailSize;
        public AnimationCurve ballSpeedToTrailSize;

        private bool meshUpdated = false;

        private void Start()
        {
            MeshFilter MF = GetComponent<MeshFilter>();
            MF.mesh = new Mesh();
            _trailMesh = MF.mesh;
            MeshRenderer MR = GetComponent<MeshRenderer>();

            MR.shadowCastingMode = ShadowCastingMode.Off;
            MR.receiveShadows = false;
        }

        public void AddNewPosition(Vector3 point, Vector3 normal, Vector3 forward, float speed, float size)
        {
            if (LastPoint != null)
            {
                Vector2 curPos = new Vector2(point.x, point.z);
                Vector2 prevPointPos = new Vector2(LastPoint.Point_Center.x, LastPoint.Point_Center.z);

                if (Vector2.Distance(curPos, prevPointPos) < PointsDistance)
                    return;
            }

            Vector3 PlacePoint = point + normal * HeightOffset;
            var hitCross = LastPoint == null
                ? Vector3.Cross(-forward, normal).normalized
                : Vector3.Cross(LastPoint.Point_Center - PlacePoint, normal).normalized;

            Vector3 Point1_Lt = PlacePoint + hitCross * ballSizeToTrailSize.Evaluate(size) *
                ballSpeedToTrailSize.Evaluate(speed) *
                (1f + UnityEngine.Random.Range(-TrackWidthVariation, TrackWidthVariation));
            Vector3 Point2_Rt = PlacePoint - hitCross * ballSizeToTrailSize.Evaluate(size) *
                ballSpeedToTrailSize.Evaluate(speed) *
                (1f + UnityEngine.Random.Range(-TrackWidthVariation, TrackWidthVariation));

            float lifetime = trailLifetime;
            if (LastPoint == null)
                lifetime = 0f;

            LastPoint = new TrailPoint(PlacePoint, Point1_Lt, Point2_Rt, new Vector2(UVPos, 0), new Vector2(UVPos, 1));

            LastPoint.lifetime = lifetime;
            _points.Add(LastPoint);

            if (_points.Count > MaxTrailPoints)
                _points.RemoveAt(0);

            UVPos += UVInc;
            if (UVPos > 1000)
                UVPos = 0;

            meshUpdated = true;
        }

        private void LateUpdate()
        {
            if (!meshUpdated && !fadeTrail)
                return;

            if (fadeTrail)
                for (int i = 0; i < _points.Count; i++)
                {
                    if (_points[i].lifetime > 0f)
                        _points[i].lifetime -= Time.deltaTime;
                }

            if (_points.Count < 3)
                return;

            Vector3[] verticies = new Vector3[_points.Count * 2];
            Vector2[] uv = new Vector2[_points.Count * 2];
            Color[] colors = new Color[_points.Count * 2];

            int[] triangles = new int[_points.Count * 6];

            float startCounter = 0;
            int counter = 0;

            for (int i = 0; i < _points.Count - 1; i++)
            {

                TrailPoint startPoint = _points[i];
                TrailPoint endPoint = _points[i + 1];

                float startFadeValue = 1f;

                if (startFadePoints > 0)
                    startFadeValue = Mathf.Clamp01(startCounter / startFadePoints);

                verticies[counter * 2] = startPoint.Point1_Lt;
                verticies[counter * 2 + 1] = startPoint.Point2_Rt;
                verticies[counter * 2 + 2] = endPoint.Point1_Lt;
                verticies[counter * 2 + 3] = endPoint.Point2_Rt;

                uv[counter * 2] = startPoint.UVPos1_Lt;
                uv[counter * 2 + 1] = startPoint.UVPos2_Rt;
                uv[counter * 2 + 2] = endPoint.UVPos1_Lt;
                uv[counter * 2 + 3] = endPoint.UVPos2_Rt;

                triangles[counter * 6 + 0] = counter * 2 + 2;
                triangles[counter * 6 + 1] = counter * 2 + 0;
                triangles[counter * 6 + 2] = counter * 2 + 1;

                triangles[counter * 6 + 3] = counter * 2 + 3;
                triangles[counter * 6 + 4] = counter * 2 + 2;
                triangles[counter * 6 + 5] = counter * 2 + 1;

                colors[i * 2].a = startPoint.lifetime / trailLifetime * startFadeValue;
                colors[i * 2 + 1].a = startPoint.lifetime / trailLifetime * startFadeValue;
                colors[i * 2 + 2].a = startPoint.lifetime / trailLifetime * startFadeValue;
                colors[i * 2 + 3].a = startPoint.lifetime / trailLifetime * startFadeValue;

                counter++;

                startCounter += 1f;

                if (startPoint.lifetime <= 0f && endPoint.lifetime <= 0f)
                    startCounter = 0f;
            }

            _trailMesh.vertices = verticies;
            _trailMesh.triangles = triangles;
            _trailMesh.uv = uv;
            _trailMesh.colors = colors;

            _trailMesh.RecalculateNormals();
            _trailMesh.RecalculateTangents();
            _trailMesh.RecalculateBounds();

            meshUpdated = false;
        }

        public float GetLifeTime()
        {
            if (_points.Count == 0)
                return 0f;
            
            var lifeTime = _points[0].lifetime;

            foreach (var point in _points)
                if (point.lifetime > lifeTime)
                    lifeTime = point.lifetime;

            return lifeTime;
        }
    }

    [Serializable]
    public class TrailPoint
    {
        public TrailPoint(Vector3 center, Vector3 point1_Lt, Vector3 point2_Rt, Vector2 uv1_lt, Vector2 uv2_rt)
        {
            Point_Center = center;
            Point1_Lt = point1_Lt;
            Point2_Rt = point2_Rt;
            UVPos1_Lt = uv1_lt;
            UVPos2_Rt = uv2_rt;
        }

        public Vector3 Point_Center;
        public Vector3 Point1_Lt;
        public Vector3 Point2_Rt;

        public Vector2 UVPos1_Lt;
        public Vector2 UVPos2_Rt;
        public float lifetime = 0f;
    }
}