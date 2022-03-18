using System;
using Unity.Entities;

namespace DOTS_Test
{
    [Serializable]
    public struct CircleColliderComponent : IComponentData
    {
        public float Radius;
    }
}
