using Unity.Burst;
using Unity.Deformations;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.TestGunAnimationSystem
{
    public partial class TestGunAnimationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            new TestGunFireJob{}.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct TestGunFireJob : IJobEntity
    {
        void Execute(
            ref TestGunAnimationComponent testGunAnimationComponent,
            ref DynamicBuffer<BlendShapeWeight> blendShapeWeightBuffer,
            ref DynamicBuffer<BlendShapeInfoData> blendShapeInfoBuffer,
            in Translation translation
        )
        {
            var readyToAnimation = testGunAnimationComponent.IsAnimation;

            if (readyToAnimation)
            {
                for (var i = 0; i < blendShapeInfoBuffer.Length; i++)
                {
                    blendShapeInfoBuffer[i] = new BlendShapeInfoData()
                    {
                        StateId = blendShapeInfoBuffer[i].StateId,
                        MinValue = blendShapeInfoBuffer[i].MinValue,
                        MaxValue = blendShapeInfoBuffer[i].MaxValue,
                        ChangeStep = blendShapeInfoBuffer[i].ChangeStep,
                        SpeedMod = 1,
                        IsAnimationCompleted = false
                    };
                }

                testGunAnimationComponent.IsAnimation = false;
            }

            var isAnimating = false;

            foreach (var blendShapeInfo in blendShapeInfoBuffer)
                if (!blendShapeInfo.IsAnimationCompleted)
                    isAnimating = true;

            if (!isAnimating)
            {
                if(testGunAnimationComponent.IsLoop)
                    testGunAnimationComponent.IsAnimation = true;
                
                return;
            }

            for (var i = 0; i < blendShapeInfoBuffer.Length; i++)
            {
                var speed = testGunAnimationComponent.AnimationSpeed;
                var id = blendShapeInfoBuffer[i].StateId;
                var changeStep = blendShapeInfoBuffer[i].ChangeStep;
                var speedMod = blendShapeInfoBuffer[i].SpeedMod;
                var maxValue = blendShapeInfoBuffer[i].MaxValue;
                var minValue = blendShapeInfoBuffer[i].MinValue;
                var isAnimated = blendShapeInfoBuffer[i].IsAnimationCompleted;

                if (isAnimated)
                    continue;
                
                for (var j = 0; j < blendShapeWeightBuffer.Length; j++)
                {
                    if (id == j)
                    {
                        var value = blendShapeWeightBuffer[j].Value;

                        value += changeStep * speedMod * speed * Mathf.Clamp(translation.Value.x, 1f, 10f);
                        blendShapeWeightBuffer[j] = new BlendShapeWeight() {Value = value};

                        if (blendShapeWeightBuffer[j].Value > maxValue && speedMod > 0)
                            blendShapeInfoBuffer[i] = new BlendShapeInfoData()
                            {
                                StateId = blendShapeInfoBuffer[i].StateId,
                                MinValue = blendShapeInfoBuffer[i].MinValue,
                                MaxValue = blendShapeInfoBuffer[i].MaxValue,
                                ChangeStep = blendShapeInfoBuffer[i].ChangeStep,
                                SpeedMod = -blendShapeInfoBuffer[i].SpeedMod,
                                IsAnimationCompleted = blendShapeInfoBuffer[i].IsAnimationCompleted
                            };

                        if (blendShapeWeightBuffer[j].Value <= minValue && speedMod < 0)
                            blendShapeInfoBuffer[i] = new BlendShapeInfoData()
                            {
                                StateId = blendShapeInfoBuffer[i].StateId,
                                MinValue = blendShapeInfoBuffer[i].MinValue,
                                MaxValue = blendShapeInfoBuffer[i].MaxValue,
                                ChangeStep = blendShapeInfoBuffer[i].ChangeStep,
                                SpeedMod = -blendShapeInfoBuffer[i].SpeedMod,
                                IsAnimationCompleted = !blendShapeInfoBuffer[i].IsAnimationCompleted
                            };
                    }
                }
            }
        }
    }
}
