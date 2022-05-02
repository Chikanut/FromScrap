using Unity.Entities;
using UnityEngine;

public class RotateToTargetAuthoring : FindTargetAuthoring
{
    [Header("Rotate Towards Settings")]
    [SerializeField] private bool VerticalRotation;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float IsRotatedRadius;

    protected override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);

        var rotatingComponent = new RotateTowardsTarget()
        {
            VerticalRotation = VerticalRotation,
            RotationSpeed = RotationSpeed,
            IsRotatedRadius = IsRotatedRadius
        };
        
        dstManager.AddComponentData(entity, rotatingComponent);
    }
}
