using Unity.Entities;
using Unity.Mathematics;

namespace ECS.TestGunAnimationSystem
{
    public struct BlendShapeInfoData : IBufferElementData
    {
        public int StateId;
        public float MinValue;
        public float MaxValue;
        public float ChangeStep;
        public float SpeedMod;
        public bool IsAnimationCompleted;
    }
}
