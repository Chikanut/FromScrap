using Unity.Entities;

namespace ECS.TestGunAnimationSystem
{
    public struct TestGunAnimationComponent : IComponentData
    {
        public float AnimationSpeed;
        public bool IsAnimation;
        public bool IsLoop;
    }
}
