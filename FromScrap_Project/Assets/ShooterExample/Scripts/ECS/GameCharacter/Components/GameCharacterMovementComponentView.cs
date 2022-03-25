using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct GameCharacterMovementComponent : IComponentData
    {
        public float HorizontalAxis { get; set; }
        public float VerticalAxis{ get; set; }
        public bool SpaceKey { get; set; }
        
        public float MaxMotorTorque { get; set; }
        public float MaxBreakTorque { get; set; }
        public float MaxSteerAngle { get; set; }
        public float MaxSpeed { get; set; }
        public float MaxAcceleration { get; set; }

        public float3 CurrentVelocity { get; set; }
        public float CurrentSpeed { get; set; }

        public float SpeedParameter { get; set; }
        public float BreakParameter { get; set; }
        public float SteerParameter { get; set; }
    }

[Serializable]
public sealed class GameCharacterMovementComponentView : MonoBehaviour, IConvertGameObjectToEntity
{
    public float maxMotorTorque = 1f;
    public float maxBreakTorque = 1f;
    public float maxSteerAngle = 1f;
    public float maxSpeed = 5f;
    public float maxAcceleration = 2f;

    public float3 currentVelocity = new float3(0f, 0f, 0f);
    public float currentSpeed = 1f;

    public float speedParameter = 1f;
    public float breakParameter = 1f;
    public float steerParameter = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }

        dstManager.AddComponentData(entity, new GameCharacterMovementComponent()
        {
           MaxMotorTorque = maxMotorTorque,
           MaxBreakTorque = maxBreakTorque,
           MaxSteerAngle = maxSteerAngle,
           MaxSpeed = maxSpeed,
           MaxAcceleration = maxAcceleration,
           CurrentVelocity = currentVelocity,
           CurrentSpeed = currentSpeed,
           SpeedParameter = speedParameter,
           BreakParameter = breakParameter,
           SteerParameter = steerParameter
        });
    }
}
