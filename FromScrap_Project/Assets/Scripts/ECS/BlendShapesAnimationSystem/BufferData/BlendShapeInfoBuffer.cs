using Unity.Entities;
using VertexFragment;

namespace ECS.BlendShapesAnimations.Components
{
    public struct BlendShapeInfoBuffer : IBufferElementData
    {
        public int StateId;
        
        public float MinValue;
        public float MaxValue;
        
        public MathUtils.LoopType LoopType;
        public float LoopTime;

        public float Duration;
        public float StartTime;
    }
}
