using ECS.BlendShapesAnimations.Components;
using Unity.Deformations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VertexFragment;

namespace ECS.BlendShapesAnimations.System
{
    public partial class BlendShapeAnimationSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            var deltaTime = Time.DeltaTime;

            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex,
                ref BlendShapeAnimationComponent animationComponent,
                ref DynamicBuffer<BlendShapeAnimationsBuffer> blendShapeAnimations,
                ref DynamicBuffer<BlendShapeWeight> blendShapeWeightBuffer,
                ref DynamicBuffer<BlendShapeInfoBuffer> blendShapeInfoBuffer) =>
            {
                switch (animationComponent.State)
                {
                    case BlendShapeAnimationState.Start:
                    {
                        for (int i = 0; i < blendShapeWeightBuffer.Length; i++)
                        {
                            blendShapeWeightBuffer[i] = new BlendShapeWeight {Value = 0};
                        }

                        animationComponent.State = BlendShapeAnimationState.Play;

                        if (blendShapeAnimations.Length <= animationComponent.AnimationIndex)
                            return;

                        var animation = blendShapeAnimations[animationComponent.AnimationIndex];
                        if (animationComponent.OverrideTime)
                        {

                            animation.Duration = animationComponent.Time;
                        }

                        animation.Progress = 0;
                        blendShapeAnimations[animationComponent.AnimationIndex] = animation;
                    }

                        break;
                    case BlendShapeAnimationState.Play:
                    {
                        if (blendShapeAnimations.Length <= animationComponent.AnimationIndex)
                            return;

                        var animation = blendShapeAnimations[animationComponent.AnimationIndex];
                        animation.Progress += deltaTime;
                        var clampedProgress = MathUtils.LoopValue(animation.LoopType, animation.Progress,
                            animation.Duration);

                        unsafe
                        {
                            for (int i = 0; i < BlendShapeAnimationsBuffer.MaxBlendShapesCount; i++)
                            {
                                if (animation.BlendShapeIndex[i] == -1)
                                    continue;

                                var blendShape = blendShapeInfoBuffer[animation.BlendShapeIndex[i]];

                                var startTime = blendShape.StartTime * animation.Duration;
                                var duration = blendShape.Duration * animation.Duration;
                                
                                if (clampedProgress < startTime || clampedProgress > duration + startTime) continue;
                                
                                var weigh = 0f;

                                if (blendShape.LoopType == MathUtils.LoopType.None)
                                {
                                    weigh = math.clamp((clampedProgress - startTime) / duration, 0, 1);
                                }
                                else
                                {
                                    weigh = MathUtils.LoopValue(blendShape.LoopType, clampedProgress - startTime,
                                    blendShape.LoopTime);
                                }
                                
                                weigh = math.lerp(blendShape.MinValue, blendShape.MaxValue, weigh);

                                var blendShapeWeight = blendShapeWeightBuffer[blendShape.StateId];
                                blendShapeWeight.Value = weigh;
                                blendShapeWeightBuffer[blendShape.StateId] = blendShapeWeight;
                            }
                        }

                        if (animation.LoopType == MathUtils.LoopType.None && animation.Progress >= animation.Duration)
                        {
                            animationComponent.State = BlendShapeAnimationState.End;
                        }

                        blendShapeAnimations[animationComponent.AnimationIndex] = animation;
                    }
                        break;
                    case BlendShapeAnimationState.End:
                        ecb.RemoveComponent<BlendShapeAnimationComponent>(entityInQueryIndex, entity);
                        break;
                }

            }).ScheduleParallel(Dependency);

            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
