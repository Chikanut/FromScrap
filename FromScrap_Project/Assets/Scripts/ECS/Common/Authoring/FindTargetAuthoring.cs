using Unity.Entities;
using UnityEngine;

public class FindTargetAuthoring : QuadrantObjectAuthoring
{
    [Header("Find Target Settings")]
    [SerializeField] private QuadrantEntityData.TypeNum TargetType;
    [SerializeField] private float Range;
    
    protected override void ConvertAncestors(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        base.ConvertAncestors(entity, dstManager, conversionSystem);
        
        var findClosestTarget = new FindTargetData()
        {
            Range = Range,
            TargetType = TargetType
        };

        dstManager.AddComponentData(entity, findClosestTarget);
        dstManager.AddComponentData(entity, new HasTarget());
    }
}
