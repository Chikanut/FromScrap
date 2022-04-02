using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
public struct GameCharacterMovementComponent : IComponentData
    {
        public float HorizontalAxis { get; set; }
        public float VerticalAxis{ get; set; }
        public bool SpaceKey { get; set; }
        public bool BoostKey { get; set; }

        public float MaxSpeed { get; set; }
        public float BoostSpeedMultiplier { get; set; }
        public float RotationSpeed { get; set; }
        public float MaxAcceleration { get; set; }
        public float JetPower { get; set; }
        public float JetPowerAcceleration { get; set; }
        public float CarStabilizationLevel { get; set; }
        public float CarStabilizationSpeed { get; set; }

        public float3 CurrentVelocity { get; set; }
        public float CurrentJetPower { get; set; }
        public float3 CurrentDirection { get; set; }
    }

[Serializable]
public sealed class GameCharacterMovementComponentView : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float boostSpeedMultiplier = 2f;
    public float rotationSpeed = 1f;
    public float maxAcceleration = 2f;
    public float jetPower = 2f;
    public float jetPowerAcceleration = 2f;
    public float CarStabilizationLevel = 0.8f;
    public float CarStabilizationSpeed = 2f;

    [Header("Current Parameters")]
    public float3 currentVelocity = new float3(0f, 0f, 0f);
    public float currentJetPower = 1f;
    public float3 currentDirection = new float3(0f, 0f, 0f);

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }

        dstManager.AddComponentData(entity, new GameCharacterMovementComponent()
        {
            MaxSpeed = maxSpeed,
            BoostSpeedMultiplier = boostSpeedMultiplier,
            RotationSpeed = rotationSpeed,
            MaxAcceleration = maxAcceleration,
            JetPower = jetPower,
            JetPowerAcceleration = jetPowerAcceleration,
            CurrentVelocity = currentVelocity,
            CurrentJetPower = currentJetPower,
            CurrentDirection = currentDirection,
            CarStabilizationLevel = CarStabilizationLevel,
            CarStabilizationSpeed = CarStabilizationSpeed
        });
    }
}
