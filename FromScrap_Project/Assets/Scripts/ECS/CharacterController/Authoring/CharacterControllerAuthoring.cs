using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using UnityEngine;
using static Unity.Physics.PhysicsStep;

public class CharacterControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float3 Gravity = Default.Gravity;
    public float MovementSpeed = 2.5f;
    public float MaxMovementSpeed = 10.0f;
    public float RotationSpeed = 2.5f;
    public float JumpUpwardsSpeed = 5.0f;
    public float MaxSlope = 60.0f;
    public int MaxIterations = 10;
    public float CharacterMass = 1.0f;
    public float SkinWidth = 0.02f;
    public float ContactTolerance = 0.1f;
    public bool AffectsPhysicsBodies = true;
    public bool RaiseCollisionEvents = false;
    public bool RaiseTriggerEvents = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            var componentData = new CharacterControllerComponentData
            {
                Gravity = Gravity,
                MovementSpeed = MovementSpeed,
                MaxMovementSpeed = MaxMovementSpeed,
                RotationSpeed = RotationSpeed,
                JumpUpwardsSpeed = JumpUpwardsSpeed,
                MaxSlope = math.radians(MaxSlope),
                MaxIterations = MaxIterations,
                CharacterMass = CharacterMass,
                SkinWidth = SkinWidth,
                ContactTolerance = ContactTolerance,
                AffectsPhysicsBodies = (byte)(AffectsPhysicsBodies ? 1 : 0),
                RaiseCollisionEvents = (byte)(RaiseCollisionEvents ? 1 : 0),
                RaiseTriggerEvents = (byte)(RaiseTriggerEvents ? 1 : 0)
            };
            var internalData = new CharacterControllerInternalData
            {
                Entity = entity,
                Input = new CharacterControllerInput(),
            };

            dstManager.AddComponentData(entity, componentData);
            dstManager.AddComponentData(entity, internalData);
            if (RaiseCollisionEvents)
            {
                dstManager.AddBuffer<StatefulCollisionEvent>(entity);
            }
            if (RaiseTriggerEvents)
            {
                dstManager.AddBuffer<StatefulTriggerEvent>(entity);
                dstManager.AddComponentData(entity, new ExcludeFromTriggerEventConversion { });
            }
        }
    }
}


