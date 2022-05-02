using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class CharacterControllerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float MaxSpeed;
    public float Acceleration;
    public float MaxAcceleration;
    public float MaxSidewaysImpulse;
    public float LevelingPower;
    public float RotationSpeed;
    
    public Vector3 GroundCheckOffset;
    public float GroundCheckRadius;
    
    public PhysicsCategoryTags BelongsTo;
    public PhysicsCategoryTags CollideWith;

    private CollisionFilter GetCollisionFilter =>
        new()
        {
            CollidesWith = CollideWith.Value,
            BelongsTo = BelongsTo.Value
        };
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (enabled)
        {
            var componentData = new CharacterControllerComponentData
            {
                MaxSpeed = MaxSpeed,
                Acceleration = Acceleration,
                MaxAcceleration = MaxAcceleration,
                MaxSidewaysImpulse = MaxSidewaysImpulse,
                LevelingPower = LevelingPower,
                RotationSpeed = RotationSpeed,
            };
            
            dstManager.AddComponentData(entity, componentData);
            dstManager.AddComponentData(entity, new CharacterControllerInput());
            dstManager.AddComponentData(entity, new GroundInfoData()
            {
                CheckDistance = GroundCheckRadius,
                CheckOffset = GroundCheckOffset,
                isLocalDown = false,
                CollisionFilter = GetCollisionFilter,
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        
        Gizmos.DrawWireSphere(transform.TransformPoint(GroundCheckOffset), GroundCheckRadius);
    }
}


