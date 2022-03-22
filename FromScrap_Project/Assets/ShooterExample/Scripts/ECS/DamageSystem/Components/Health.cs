using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct HealthComponent : IComponentData
    {
        public int Value;
    }
}
