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
        public float TurnSpeed { get; set; }
        public float MaxAcceleration { get; set; }
        public float JetPower { get; set; }
        public float JetPowerAcceleration { get; set; }
        public float CarLongitudinalStabilizationStartLevel { get; set; }
        public float CarLongitudinalStabilizationEndLevel { get; set; }
        public float CarLateralStabilizationStartLevel { get; set; }
        public float CarLateralStabilizationEndLevel { get; set; }
        public float CarLongitudinalStabilizationForce { get; set; }
        public float CarLateralStabilizationForce { get; set; }

        public float3 CurrentVelocity { get; set; }
        public float CurrentJetPower { get; set; }
        public float3 CurrentDirection { get; set; }
        public bool IsLongitudinalStabilization { get; set; }
        public bool IsLateralStabilization { get; set; }
        public float CurrentSpeedModificator { get; set; }
        public float CurrentTurnSpeedModificator { get; set; }
    }

[Serializable]
public sealed class GameCharacterMovementComponentView : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Movement Settings")]
    public float maxSpeed = 300f;
    public float boostSpeedMultiplier = 2f;
    public float turnSpeed = 20f;
    public float maxAcceleration = 5f;
    public float jetPower = 100f;
    public float jetPowerAcceleration = 2f;
    public float carLongitudinalStabilizationStartLevel = 0.75f;
    public float carLongitudinalStabilizationEndLevel = 0.85f;
    public float carLateralStabilizationStartLevel = 0.65f;
    public float carLateralStabilizationEndLevel = 0.8f;
    public float carLongitudinalStabilizationForce = 200f;
    public float carLateralStabilizationForce = 100f;

    [Header("Current Parameters")]
    public float3 currentVelocity = new float3(0f, 0f, 0f);
    public float currentJetPower = 0f;
    public float3 currentDirection = new float3(0f, 0f, 0f);
    public bool isLongitudinalStabilization = false;
    public bool isLateralStabilization = false;
    public float currentSpeedModificator = 1f;
    public float currentTurnSpeedModificator = 1f;

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
            TurnSpeed = turnSpeed,
            MaxAcceleration = maxAcceleration,
            JetPower = jetPower,
            JetPowerAcceleration = jetPowerAcceleration,
            CurrentVelocity = currentVelocity,
            CurrentJetPower = currentJetPower,
            CurrentDirection = currentDirection,
            IsLongitudinalStabilization = isLongitudinalStabilization,
            IsLateralStabilization = isLateralStabilization,
            CarLongitudinalStabilizationStartLevel = carLongitudinalStabilizationStartLevel,
            CarLongitudinalStabilizationEndLevel = carLongitudinalStabilizationEndLevel,
            CarLateralStabilizationStartLevel = carLateralStabilizationStartLevel,
            CarLateralStabilizationEndLevel = carLateralStabilizationEndLevel,
            CarLongitudinalStabilizationForce = carLongitudinalStabilizationForce,
            CarLateralStabilizationForce = carLateralStabilizationForce,
            CurrentSpeedModificator = currentSpeedModificator,
            CurrentTurnSpeedModificator = currentTurnSpeedModificator
        });
    }
}
