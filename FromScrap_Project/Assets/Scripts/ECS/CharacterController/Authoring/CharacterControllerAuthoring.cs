using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using UnityEngine;
using static Unity.Physics.PhysicsStep;

public class CharacterControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    // Gravity force applied to the character controller body
    public float3 Gravity = Default.Gravity;

    // Speed of movement initiated by user input
    public float MovementSpeed = 2.5f;

    // Maximum speed of movement at any given time
    public float MaxMovementSpeed = 10.0f;

    // Speed of rotation initiated by user input
    public float RotationSpeed = 2.5f;

    // Speed of upwards jump initiated by user input
    public float JumpUpwardsSpeed = 5.0f;

    // Maximum slope angle character can overcome (in degrees)
    public float MaxSlope = 60.0f;

    // Maximum number of character controller solver iterations
    public int MaxIterations = 10;

    // Mass of the character (used for affecting other rigid bodies)
    public float CharacterMass = 1.0f;

    // Keep the character at this distance to planes (used for numerical stability)
    public float SkinWidth = 0.02f;

    // Anything in this distance to the character will be considered a potential contact
    // when checking support
    public float ContactTolerance = 0.1f;

    // Whether to affect other rigid bodies
    public bool AffectsPhysicsBodies = true;

    // Whether to raise collision events
    // Note: collision events raised by character controller will always have details calculated
    public bool RaiseCollisionEvents = false;

    // Whether to raise trigger events
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


