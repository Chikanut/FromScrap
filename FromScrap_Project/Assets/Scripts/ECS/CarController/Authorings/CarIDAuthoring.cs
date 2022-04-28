using Cars.View.Components;
using EditorTools;
using Unity.Entities;
using UnityEngine;

public class CarIDAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [ReadOnly]public int ID;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CarIDComponent() {ID = ID});
    }
}
