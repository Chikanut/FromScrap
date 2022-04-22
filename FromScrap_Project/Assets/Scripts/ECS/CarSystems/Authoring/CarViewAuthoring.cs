using System.Collections.Generic;
using Cars.View.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;


namespace Cars.View.Authorings
{
    public class CarViewAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class Wheel
        {
            public GameObject WheelObject;

            public float WheelSkinWidth;
            public float Radius;
            public float SuspensionDistance;
            public float SuspensionOffset;
            public float SuspensionDamping;
            
            public bool isGuide;
            public bool isLeft;
            [Range(0, 1)]
            public float TurnRange;
            public float TurnDamping;

            public PhysicsCategoryTags BelongsTo;
            public PhysicsCategoryTags CollideWith;
        }

        [System.Serializable]
        public class Body
        {
            public GameObject BodyObject;
            
            [Header("Collision Check")]
            public float CarHeigh;
            public float2 CarSize;
        
            private float3[] BodyPoints;
            public PhysicsCategoryTags BelongsTo;
            public PhysicsCategoryTags CollideWith;

            [Header("Body Settings")]
            public float RotationDamping;
            public float SuspensionDamping;
            public float SuspensionRange = 0.2f;
        }

        [Header("Car view parts")]
        [SerializeField] private Body _body;
        [SerializeField] private List<Wheel> _wheels = new List<Wheel>();
        [SerializeField] private GameObject TrailObject;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //Init Wheels
            foreach (var wheel in _wheels)
            {
                if (wheel.WheelObject.transform.parent == null)
                {
                    Debug.LogError("Wheel has no parent to connect to!");
                    continue;
                }

                var wheelEntity = conversionSystem.GetPrimaryEntity(wheel.WheelObject);
                var parent = dstManager.GetComponentData<Parent>(wheelEntity).Value;

                var wheelData = new WheelData()
                {
                    Parent = parent,
                    Radius = wheel.Radius,
                    SuspensionDistance = wheel.SuspensionDistance,
                    SuspensionOffset = wheel.SuspensionOffset,
                    SuspensionDamping = wheel.SuspensionDamping,
                    isGuide = wheel.isGuide,
                    TurnRange = wheel.TurnRange,
                    TurnDamping = wheel.TurnDamping,
                    LocalAnchor = wheel.WheelObject.transform.localPosition,
                    isLeft = wheel.isLeft
                };

                var checkGround = new GroundInfoData()
                {
                    CheckDistance = wheel.Radius + wheel.WheelSkinWidth,
                    CollisionFilter = new CollisionFilter()
                    {
                        CollidesWith = wheel.CollideWith.Value,
                        BelongsTo = wheel.BelongsTo.Value
                    }
                };

                dstManager.AddComponentData(wheelEntity, wheelData);
                dstManager.AddComponentData(wheelEntity, checkGround);

                if (TrailObject != null)
                    EntityPoolManager.Instance.GetObject(TrailObject, (entity, manager) =>
                    {
                        manager.AddComponentData(entity,
                            new GeometryTrailEffectData()
                            {
                                TargetEntity = wheelEntity
                            });

                        var trailEffectInfoBuffer = manager.AddBuffer<GeometryTrailEffectInfoData>(entity);

                        trailEffectInfoBuffer.Add(new GeometryTrailEffectInfoData()
                        {
                            Point_Center = float3.zero,
                            Point1_Lt = float3.zero,
                            Point2_Rt = float3.zero,
                            UVPos1_Lt = float2.zero,
                            UVPos2_Rt = float2.zero,
                            Lifetime = 0f
                        });

                        var trailEffectLastInfoBuffer = manager.AddBuffer<GeometryTrailEffectLastInfoData>(entity);

                        trailEffectLastInfoBuffer.Add(new GeometryTrailEffectLastInfoData()
                        {
                            Point_Center = float3.zero,
                            Point1_Lt = float3.zero,
                            Point2_Rt = float3.zero,
                            UVPos1_Lt = float2.zero,
                            UVPos2_Rt = float2.zero,
                            Lifetime = 0f,
                            IsActive = false
                        });
                    });
            }

            //Init Body
            if (_body.BodyObject.transform.parent == null)
            {
                Debug.LogError("Wheel has no parent to connect to!");
            }
            else
            {
                var bodyEntity = conversionSystem.GetPrimaryEntity(_body.BodyObject);
                var parent = dstManager.GetComponentData<Parent>(bodyEntity).Value;
            
                var bodyInfo = new CarBodyData()
                {
                    Parent = parent,
                    RotationDamping = _body.RotationDamping,
                    SuspensionDamping = _body.SuspensionDamping,
                    CurrentForward = math.forward(),
                    CurrentUp = math.up(),
                    CurrentSuspension = float3.zero,
                    SuspensionRange = _body.SuspensionRange
                };
                
                dstManager.AddComponentData(bodyEntity, bodyInfo);
                var buffer = dstManager.AddBuffer<MultyGroundInfoData>(parent);

                var BodyPoints = new[]
                {
                    new float3(_body.CarSize.x, 0, _body.CarSize.y),
                    new float3(-_body.CarSize.x, 0, _body.CarSize.y),
                    new float3(-_body.CarSize.x, 0, -_body.CarSize.y),
                    new float3(_body.CarSize.x, 0, -_body.CarSize.y)
                };
                
                for (int i = 0; i < BodyPoints.Length; i++)
                {
                    buffer.Add(new MultyGroundInfoData()
                    {
                        AnchorPoints = BodyPoints[i],
                        CheckDistance = _body.CarHeigh,
                        CollisionFilter = new CollisionFilter()
                        {
                            CollidesWith = _body.CollideWith.Value,
                            BelongsTo = _body.BelongsTo.Value
                        },
                        isLocalDown = true
                    });
                }
            }
        }
        
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawBodyInfo();
            DrawWheelsInfo();
        }

        void DrawWheelsInfo()
        {
            foreach (var wheel in _wheels)
            {
                DrawWheelInfo(wheel);
            }
        }

        void DrawWheelInfo(Wheel wheel)
        {
            if(wheel.WheelObject == null)
                return;
            
            Gizmos.color = Color.green;
            
            if (wheel.WheelObject.transform.parent == null)
            {
                Gizmos.color = Color.red;
                
                Gizmos.DrawLine(wheel.WheelObject.transform.position - wheel.WheelObject.transform.forward * wheel.Radius + Vector3.down * wheel.Radius,
                    wheel.WheelObject.transform.position + wheel.WheelObject.transform.forward * wheel.Radius + Vector3.up * wheel.Radius);
                
                Gizmos.DrawLine(transform.position + wheel.WheelObject.transform.forward * wheel.Radius + Vector3.down * wheel.Radius,
                    wheel.WheelObject.transform.position - wheel.WheelObject.transform.forward * wheel.Radius + Vector3.up * wheel.Radius);
                
                Debug.LogWarning("Wheel should have parent transform!");
                
                return;
            }
            
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(wheel.WheelObject.transform.position, wheel.WheelObject.transform.right, wheel.Radius);
            var startPoint = wheel.WheelObject.transform.position + wheel.WheelObject.transform.parent.up * wheel.SuspensionOffset;
            Gizmos.DrawLine(startPoint,startPoint+wheel.WheelObject.transform.parent.up*wheel.SuspensionDistance);
        }

        void DrawBodyInfo()
        {
            if(_body.BodyObject == null)
                return;
            
            if (_body.BodyObject.transform.parent == null)
            {
                DrawBodyCross(_body.BodyObject.transform.TransformPoint(new Vector3(_body.CarSize.x,0,_body.CarSize.y)));
                DrawBodyCross(_body.BodyObject.transform.TransformPoint(new Vector3(-_body.CarSize.x,0,_body.CarSize.y)));
                DrawBodyCross(_body.BodyObject.transform.TransformPoint(new Vector3(_body.CarSize.x,0,-_body.CarSize.y)));
                DrawBodyCross(_body.BodyObject.transform.TransformPoint(new Vector3(-_body.CarSize.x,0,-_body.CarSize.y)));
                
                Debug.LogWarning("Wheel should have parent transform!");
                
                return;
            }
            
            DrawBodyLineDown(_body.BodyObject.transform.TransformPoint(new Vector3(_body.CarSize.x,0,_body.CarSize.y)));
            DrawBodyLineDown(_body.BodyObject.transform.TransformPoint(new Vector3(-_body.CarSize.x,0,_body.CarSize.y)));
            DrawBodyLineDown(_body.BodyObject.transform.TransformPoint(new Vector3(_body.CarSize.x,0,-_body.CarSize.y)));
            DrawBodyLineDown(_body.BodyObject.transform.TransformPoint(new Vector3(-_body.CarSize.x,0,-_body.CarSize.y)));
        }

        void DrawBodyCross(Vector3 startPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint - _body.BodyObject.transform.forward * _body.CarHeigh/2 + Vector3.down * _body.CarHeigh/2,
                startPoint + _body.BodyObject.transform.forward * _body.CarHeigh/2 + Vector3.up * _body.CarHeigh/2);

            Gizmos.DrawLine(startPoint + _body.BodyObject.transform.forward * _body.CarHeigh/2 + Vector3.down * _body.CarHeigh/2,
                startPoint - _body.BodyObject.transform.forward * _body.CarHeigh/2 + Vector3.up * _body.CarHeigh/2);
        }

        void DrawBodyLineDown(Vector3 startPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint, startPoint - _body.BodyObject.transform.parent.up * _body.CarHeigh);
        }
    #endif
    }
}
