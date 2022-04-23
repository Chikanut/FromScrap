using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct TrailEffectComponent : IComponentData
{
    //public Transform TrackObject { get; set; }
}

[Serializable]
public class TrailEffectViewComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public Transform trackObject;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }

        dstManager.AddComponentData(entity, new TrailEffectComponent()
        {
            //TrackObject = trackObject
        });
    }
}
