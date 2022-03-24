using Unity.Entities;
using UnityEngine;

public class QuadrantObjectAuthoring : MonoBehaviour , IConvertGameObjectToEntity
{
    [Header("Quadrant Entity Settings")]
    [SerializeField] private QuadrantEntityData.TypeNum EntityType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        ConvertFindTarget(entity, dstManager, conversionSystem);
    }
    
    protected virtual void ConvertFindTarget(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        var quadrantEntityData = new QuadrantEntityData()
        {
            Type = EntityType
        };

        dstManager.AddComponentData(entity, quadrantEntityData);
    }
}
