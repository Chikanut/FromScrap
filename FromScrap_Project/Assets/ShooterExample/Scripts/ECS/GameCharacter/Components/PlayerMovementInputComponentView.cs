using System;
using Unity.Entities;
using UnityEngine;

public struct PlayerMovementInputComponent : IComponentData
    {
       
    }

[Serializable]
public sealed class PlayerMovementInputComponentView : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }

        dstManager.AddComponentData(entity, new PlayerMovementInputComponent());
    }
}
