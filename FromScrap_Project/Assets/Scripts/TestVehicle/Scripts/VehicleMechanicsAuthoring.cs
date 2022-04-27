using System.Collections.Generic;
using Demos;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class VehicleMechanicsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Chassis Parameters...")]
    public float3 chassisUp = new float3(0, 1, 0);
    public float3 chassisRight = new float3(1, 0, 0);
    public float3 chassisForward = new float3(0, 0, 1);
    
    [Header("Wheel Parameters")]
    public float wheelBase = 0.5f;
    public float wheelFrictionRight = 0.5f;
    public float wheelFrictionForward = 0.5f;
    public float wheelMaxImpulseRight = 10.0f;
    public float wheelMaxImpulseForward = 10.0f;
    
    [Header("Suspension Parameters")] 
    public float suspensionLength = 0.5f;
    public float suspensionStrength = 1.0f;
    public float suspensionDamping = 0.1f;
    
    [Header("Collision Settings")]
    public PhysicsCategoryTags BelongsTo;
    public PhysicsCategoryTags CollideWith;

    [Header("Drive Settings")] 
    public float MaxSpeed = 10;
    
    [Header("Wheels")] 
    public List<GameObject> wheels;
    public List<GameObject> steeringWheels;
    public List<GameObject> driveWheels;
    

    [Header("Miscellaneous Parameters...")]
    public bool drawDebugInformation;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new VehicleMechanics()
        {
            chassisUp = chassisUp,
            chassisForward = chassisForward,
            chassisRight = chassisRight,
            wheelBase = wheelBase,
            wheelFrictionRight = wheelFrictionRight,
            wheelFrictionForward = wheelFrictionForward,
            wheelMaxImpulseRight = wheelMaxImpulseRight,
            wheelMaxImpulseForward = wheelMaxImpulseForward,
            suspensionLength = suspensionLength,
            suspensionStrength = suspensionStrength,
            suspensionDamping = suspensionDamping,
            drawDebugInformation = drawDebugInformation,
            collisionFilter =  new CollisionFilter()
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
