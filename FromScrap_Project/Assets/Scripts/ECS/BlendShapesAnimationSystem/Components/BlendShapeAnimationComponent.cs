using Unity.Entities;

namespace ECS.BlendShapesAnimations.Components
{
    public enum BlendShapeAnimationState
    {
        Start,
        Play,
        End
    }

    [GenerateAuthoringComponent]
    public struct BlendShapeAnimationComponent : IComponentData
    {
        public int AnimationIndex;
        public bool OverrideTime;
        public float Time;
        public BlendShapeAnimationState State;
    }
}