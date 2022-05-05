using System;
using System.Collections.Generic;
using ECS.BlendShapesAnimations.Components;
using GD.MinMaxSlider;
using MyBox;
using Unity.Entities;
using UnityEngine;
using VertexFragment;


public class BlendShapeAnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Gun Fire Settings")]
    [SerializeField] private List<AnimationData> AnimationsData = new List<AnimationData>();
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!enabled)
        {
            return;
        }
        
        dstManager.AddBuffer<BlendShapeInfoBuffer>(entity);
        var animationBuffer = dstManager.AddBuffer<BlendShapeAnimationsBuffer>(entity);
        
        var currentBlendShapeIndex = 0;
        
        foreach (var data in AnimationsData)
        {
            var animationData = new BlendShapeAnimationsBuffer
            {
                Duration = data.Duration,
                LoopType = data.LoopType,
            };
            
            //Maximum blend shapes count is MaxBlendShapesCount
            for (var j = 0; j < BlendShapeAnimationsBuffer.MaxBlendShapesCount; j++)
            {
                if (data.BlendShapes.Count > j)
                {
                    dstManager.GetBuffer<BlendShapeInfoBuffer>(entity).Add(new BlendShapeInfoBuffer()
                    {
                        StateId = data.BlendShapes[j].StateId,
                        MinValue = data.BlendShapes[j].MinValue,
                        MaxValue = data.BlendShapes[j].MaxValue,
                        LoopType = data.BlendShapes[j].LoopType,
                        Duration = data.BlendShapes[j].Duration.y - data.BlendShapes[j].Duration.x,
                        StartTime = data.BlendShapes[j].Duration.x,
                        LoopTime = data.BlendShapes[j].AnimationTime
                    });
                    unsafe
                    {
                        animationData.BlendShapeIndex[j] = currentBlendShapeIndex;
                    }

                    currentBlendShapeIndex++;
                }
                else
                {
                    unsafe
                    {
                        animationData.BlendShapeIndex[j] = -1;
                    }
                }
            }
        
            animationBuffer.Add(animationData);
        }
    }

    [Serializable]
    public class AnimationData
    {
        public MathUtils.LoopType LoopType;
        public float Duration;

        public List<BlendShapeAnimationData> BlendShapes = new List<BlendShapeAnimationData>();
    }

    [Serializable]
    public class BlendShapeAnimationData
    {
        public int StateId = 0;
        
        [Range(0f, 100f)]
        public float MinValue = 0f;

        [Range(0f, 100f)] public float MaxValue = 100f;
        
        [MinMaxSlider(0,1)] public Vector2 Duration = new Vector2(0,1);
        public MathUtils.LoopType LoopType;
        [ConditionalField("LoopType",false,MathUtils.LoopType.Loop, MathUtils.LoopType.PingPong)] public float AnimationTime;
    }
}
