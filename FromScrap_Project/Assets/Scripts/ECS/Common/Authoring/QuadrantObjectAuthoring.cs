using ECS.Common;
using Unity.Entities;
using UnityEngine;

public class QuadrantObjectAuthoring : MonoBehaviour , IConvertGameObjectToEntity
{
    [Header("Quadrant Entity Settings")]
    [SerializeField] private EntityObjectType EntityType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        ConvertAncestors(entity, dstManager, conversionSystem);
    }
    
    protected virtual void ConvertAncestors(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        var quadrantEntityData = new ObjectTypeComponent()
        {
            Type = EntityType
        };

        dstManager.AddComponentData(entity, new QuadrantHashKey());
        dstManager.AddSharedComponentData(entity, quadrantEntityData);
    }
}
