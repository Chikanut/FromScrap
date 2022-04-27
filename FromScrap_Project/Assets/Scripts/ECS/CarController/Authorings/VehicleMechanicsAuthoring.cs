using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using Vehicles.Components;

namespace Vehicles.Authoring
{
    public class VehicleMechanicsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Wheel Parameters")] public float wheelBase = 0.5f;
        public float wheelFrictionRight = 0.5f;
        public float wheelFrictionForward = 0.5f;
        public float wheelMaxImpulseRight = 10.0f;
        public float wheelMaxImpulseForward = 10.0f;

        [Header("Suspension Parameters")] public float suspensionLength = 0.5f;
        public float suspensionStrength = 1.0f;
        public float suspensionDamping = 0.1f;

        [Header("Collision Settings")]
        public PhysicsCategoryTags BelongsTo;
        public PhysicsCategoryTags CollideWith;

        [Header("Drive Settings")] 
        public float MaxAcceleration = 10;
        public float MaxSteerAngle = 45;

        [Header("Wheels")]
        public List<GameObject> wheels;
        public List<GameObject> steeringWheels;
        public List<GameObject> driveWheels;


        [Header("Miscellaneous Parameters...")]
        public bool drawDebugInformation;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new VehicleSettingsComponent()
            {
                WheelBase = wheelBase,
                WheelFrictionRight = wheelFrictionRight,
                WheelFrictionForward = wheelFrictionForward,
                WheelMaxImpulseRight = wheelMaxImpulseRight,
                WheelMaxImpulseForward = wheelMaxImpulseForward,
                SuspensionLength = suspensionLength,
                SuspensionStrength = suspensionStrength,
                SuspensionDamping = suspensionDamping,
                DrawDebugInformation = drawDebugInformation,
                MaxAcceleration = MaxAcceleration,
                MaxSteerAngle = MaxSteerAngle,
                CollisionFilter = new CollisionFilter()
                {
                    CollidesWith = CollideWith.Value,
                    BelongsTo = BelongsTo.Value
                }
            });

            var wheelsBuffer = dstManager.AddBuffer<WheelsBuffer>(entity);
            for (int i = 0; i < wheels.Count; i++)
            {
                wheelsBuffer.Add(new WheelsBuffer()
                {
                    Wheel = conversionSystem.GetPrimaryEntity(wheels[i]),
                    isDriven = driveWheels.Contains(wheels[i]),
                    isStearing = steeringWheels.Contains(wheels[i])
                });
            }
        }
    }
}
