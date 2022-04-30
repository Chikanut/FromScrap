using System;
using System.Collections;
using System.Collections.Generic;
using ECS.TestGunAnimationSystem;
using Unity.Entities;
using UnityEngine;

public class TestGunAnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Gun Fire Settings")]
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private List<BlendShapeAnimationData> blendShapeAnimationsData = new List<BlendShapeAnimationData>();
    [SerializeField] private bool isAnimation = false;
    [SerializeField] private bool isLoop = false;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }

        dstManager.AddComponentData(entity, new TestGunAnimationComponent()
        {
            AnimationSpeed = animationSpeed,
            IsAnimation = isAnimation,
            IsLoop = isLoop
        });

        var blendShapeAnimationBufferData = dstManager.AddBuffer<BlendShapeInfoData>(entity);
        
        foreach (var blendShapeAnimationData in blendShapeAnimationsData)
        {
            blendShapeAnimationBufferData.Add(new BlendShapeInfoData()
            {
                StateId = blendShapeAnimationData.StateId,
                MinValue = blendShapeAnimationData.MinValue,
                MaxValue = blendShapeAnimationData.MaxValue,
                ChangeStep = blendShapeAnimationData.ChangeStep,
                SpeedMod = 1f,
                IsAnimationCompleted = false
            });
        }
    }

    [Serializable]
    public class BlendShapeAnimationData
    {
        public int StateId;
        public float MinValue;
        public float MaxValue;
        public float ChangeStep;
    }
}
