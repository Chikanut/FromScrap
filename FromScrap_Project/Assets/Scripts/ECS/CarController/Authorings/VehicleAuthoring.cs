using System;
using System.Collections.Generic;
using MyBox;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;
using Vehicles.Components;
using Vehicles.Wheels.Components;


namespace Vehicles.Wheels.Authorings
{
    public class VehicleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class WheelSize
        {
            public float WheelSkinWidth = 0.1f;
            public float Radius = 0.3f;
        }

        [System.Serializable]
        public class WheelSuspension
        {
            public float SuspensionDistance = 0.2f;
            public float SuspensionDamping = 25f;
            public float SuspensionStrength = 250;
        }

        [System.Serializable]
        public class WheelDrive
        {
            public float MaxSpeed = 25;
            public float Acceleration = 4;
            public float MaxAcceleration = 50;
            public float MaxSidewaysImpulse = 15;
        }

        [System.Serializable]
        public class WheelView
        {
            [Range(0, 1)] public float TurnRange = 0.5f;
            public float TurnDamping = 0.1f;
            public GameObject TrailObject;
        }

        [System.Serializable]
        public class WheelPhysics
        {
            public PhysicsCategoryTags BelongsTo;
            public PhysicsCategoryTags CollideWith;

            public CollisionFilter GetCollisionFilter =>
                new CollisionFilter()
                {
                    CollidesWith = CollideWith.Value,
                    BelongsTo = BelongsTo.Value
                };
        }

        [System.Serializable]
        public class Wheel
        {
            [Header("Object")] public GameObject WheelObject;

            public bool OverrideSize;
            [ConditionalField("OverrideSize")] public WheelSize OverridedSize;

            public bool OverrideSuspension;

            [ConditionalField("OverrideSuspension")]
            public WheelSuspension OverridedSuspension;

            [Header("Drive")] public bool isGuide;
            public bool isDrive;
            public bool OverrideDrive;
            [ConditionalField("OverrideDrive")] public WheelDrive OverridedDrive;

            [Space] public bool OverrideView;
            [ConditionalField("OverrideView")] public WheelView OverridedView;

            [Space] public bool OverridePhysics;
            [ConditionalField("OverridePhysics")] public WheelPhysics OverridedPhysics;

            [Space] public bool OverrideTrail;
            [ConditionalField("OverrideTrail")] public GameObject WheelTrailObject;
        }

        [Header("Default wheels data")] [SerializeField]
        private WheelSize _defaultWheelSize;

        [SerializeField] private WheelSuspension _defaultWheelsSuspension;
        [SerializeField] private WheelDrive _defaultWheelsDrive;
        [SerializeField] private WheelView _defaultView;
        [SerializeField] private WheelPhysics _defaultWheelsPhysics;
        [Header("Wheels")] [SerializeField] private List<Wheel> _wheels = new List<Wheel>();

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
                var body = conversionSystem.GetPrimaryEntity(gameObject);

                var wheelData = new ViewData()
                {
                    Body = body,
                    Parent = parent,
                    Radius = wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius,
                    IsGuide = wheel.isGuide,
                    IsDrive = wheel.isDrive,
                    TurnRange = wheel.OverrideView ? wheel.OverridedView.TurnRange : _defaultView.TurnRange,
                    TurnDamping = wheel.OverrideView ? wheel.OverridedView.TurnDamping : _defaultView.TurnDamping,
                };

                var suspensionData = new SuspensionData()
                {
                    Body = body,
                    Parent = parent,
                    SuspensionDistance =
                        Vector3.Distance(wheel.WheelObject.transform.position,
                            wheel.WheelObject.transform.parent.position) + (wheel.OverrideSuspension
                            ? wheel.OverridedSuspension.SuspensionDistance
                            : _defaultWheelsSuspension.SuspensionDistance),
                    SuspensionStrength = wheel.OverrideSuspension
                        ? wheel.OverridedSuspension.SuspensionStrength
                        : _defaultWheelsSuspension.SuspensionStrength,
                    SuspensionDamping = wheel.OverrideSuspension
                        ? wheel.OverridedSuspension.SuspensionDamping
                        : _defaultWheelsSuspension.SuspensionDamping,
                    Radius = wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius
                };

                var driveData = new DriveData()
                {
                    Body = body,
                    Parent = parent,
                    IsDrive = wheel.isDrive,
                    IsGuide = wheel.isGuide,
                    MaxSpeed = wheel.OverrideDrive
                        ? wheel.OverridedDrive.MaxSpeed
                        : _defaultWheelsDrive.MaxSpeed,
                    Acceleration = wheel.OverrideDrive ? wheel.OverridedDrive.Acceleration : _defaultWheelsDrive.Acceleration,
                    MaxAcceleration = wheel.OverrideDrive ? wheel.OverridedDrive.MaxAcceleration : _defaultWheelsDrive.MaxAcceleration,
                    MaxSidewaysImpulse = wheel.OverrideDrive
                        ? wheel.OverridedDrive.MaxSidewaysImpulse
                        : _defaultWheelsDrive.MaxSidewaysImpulse,
                };

                var checkGround = new GroundInfoData()
                {
                    CheckDistance = (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius) +
                                    (wheel.OverrideSize
                                        ? wheel.OverridedSize.WheelSkinWidth
                                        : _defaultWheelSize.WheelSkinWidth) +
                                    Vector3.Distance(wheel.WheelObject.transform.position,
                                        wheel.WheelObject.transform.parent.position) + (wheel.OverrideSuspension
                                        ? wheel.OverridedSuspension.SuspensionDistance
                                        : _defaultWheelsSuspension.SuspensionDistance),
                    CollisionFilter = (wheel.OverridePhysics ? wheel.OverridedPhysics : _defaultWheelsPhysics)
                        .GetCollisionFilter,
                    isLocalDown = true
                };

                dstManager.AddComponentData(wheelEntity, wheelData);
                dstManager.AddComponentData(wheelEntity, suspensionData);
                dstManager.AddComponentData(wheelEntity, driveData);
                dstManager.AddComponentData(parent, checkGround);

                //init wheel trail effect
                if (wheel.WheelTrailObject != null)
                {
                    var wheelTrailComponent =
                        new GameObjectTrackEntityComponent().Init(wheel.OverrideTrail
                            ? wheel.WheelTrailObject
                            : _defaultView.TrailObject);

                    dstManager.AddComponentData(wheelEntity, wheelTrailComponent);
                }
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
            if (wheel.WheelObject == null)
                return;

            Gizmos.color = Color.green;

            if (wheel.WheelObject.transform.parent == null)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(
                    wheel.WheelObject.transform.position - wheel.WheelObject.transform.forward *
                    (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius) +
                    Vector3.down * (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius),
                    wheel.WheelObject.transform.position + wheel.WheelObject.transform.forward *
                    (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius) +
                    Vector3.up * (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius));

                Gizmos.DrawLine(
                    transform.position + wheel.WheelObject.transform.forward *
                    (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius) +
                    Vector3.down * (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius),
                    wheel.WheelObject.transform.position - wheel.WheelObject.transform.forward *
                    (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius) +
                    Vector3.up * (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius));

                Debug.LogWarning("Wheel should have parent transform!");

                return;
            }

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(wheel.WheelObject.transform.position, wheel.WheelObject.transform.right,
                (wheel.OverrideSize ? wheel.OverridedSize.Radius : _defaultWheelSize.Radius));
            var startPoint = wheel.WheelObject.transform.parent.position;
            Gizmos.DrawLine(startPoint,
                startPoint - wheel.WheelObject.transform.parent.up *
                (Vector3.Distance(wheel.WheelObject.transform.position, wheel.WheelObject.transform.parent.position) +
                 (wheel.OverrideSuspension
                     ? wheel.OverridedSuspension.SuspensionDistance
                     : _defaultWheelsSuspension.SuspensionDistance)));
        }

#endif
    }
}
