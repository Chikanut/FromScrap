using Unity.Entities;
using MathUtils = VertexFragment.MathUtils;

namespace ECS.BlendShapesAnimations.Components
{
    public unsafe struct BlendShapeAnimationsBuffer : IBufferElementData
    {
        public const int MaxBlendShapesCount = 5;
        
        public float Duration;
        public MathUtils.LoopType LoopType;
        public fixed int BlendShapeIndex[MaxBlendShapesCount];

        public float Progress;
    }
}
