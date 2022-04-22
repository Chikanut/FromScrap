using Unity.Entities;
using UnityEngine;

public class LinksAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var links = dstManager.AddBuffer<LinkedEntityGroup>(entity);

        links.Add(new LinkedEntityGroup() {Value = entity});
        for (int i = 0; i < transform.childCount; i++)
        {
            links.Add(new LinkedEntityGroup() {Value = conversionSystem.GetPrimaryEntity(transform.GetChild(i).gameObject)});
        }
    }
}
