using System.Collections.Generic;
using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;
using Vehicles.Components;


namespace Cars.View.Authorings
{
    public class VehicleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class Wheel
        {
            public GameObject WheelObject;

            public float WheelSkinWidth;
            public float Radius;
            public float SuspensionDistance;
            public float SuspensionDamping;
            public float SuspensionStrength;
            
            public bool isGuide;
            [Range(0, 1)]
            public float TurnRange;
            public float TurnDamping;

            public PhysicsCategoryTags BelongsTo;
            public PhysicsCategoryTags CollideWith;

            public GameObject WheelTrailObject;
        }
        
        [Header("Car view parts")]
        [SerializeField] private List<Wheel> _wheels = new List<Wheel>();
        [SerializeField] private GameObject TrailObject;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new VehicleInputComponent());
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
                    Body = conversionSystem.GetPrimaryEntity(gameObject),
                    Parent = parent,
                    Radius = wheel.Radius,
                    SuspensionDistance = Vector3.Distance(wheel.WheelObject.transform.position, wheel.WheelObject.transform.parent.position) + wheel.SuspensionDistance,
                    SuspensionStrength = wheel.SuspensionStrength,
                    SuspensionDamping = wheel.SuspensionDamping,
                    isGuide = wheel.isGuide,
                    TurnRange = wheel.TurnRange,
                    TurnDamping = wheel.TurnDamping,
                };

                var checkGround = new GroundInfoData()
                {
                    CheckDistance = wheel.Radius + wheel.WheelSkinWidth + Vector3.Distance(wheel.WheelObject.transform.position, wheel.WheelObject.transform.parent.position)+ wheel.SuspensionDistance,
                    CollisionFilter = new CollisionFilter()
                    {
                        CollidesWith = wheel.CollideWith.Value,
                        BelongsTo = wheel.BelongsTo.Value
                    },
                    isLocalDown = true
                };

                dstManager.AddComponentData(wheelEntity, wheelData);
                dstManager.AddComponentData(parent, checkGround);

                //init wheel trail effect
                if (wheel.WheelTrailObject != null)
                {
                    var wheelTrailComponent = new GameObjectTrackEntityComponent().Init(wheel.WheelTrailObject);

                    dstManager.AddComponentData(wheelEntity, wheelTrailComponent);
                }

                //Init geometry car trail system
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
        }
        
    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
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
            var startPoint = wheel.WheelObject.transform.parent.position;
            Gizmos.DrawLine(startPoint, startPoint - wheel.WheelObject.transform.parent.up * (Vector3.Distance(wheel.WheelObject.transform.position, wheel.WheelObject.transform.parent.position)+ wheel.SuspensionDistance));
        }

    #endif
    }
}
