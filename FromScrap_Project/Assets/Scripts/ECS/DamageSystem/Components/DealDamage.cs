using System;
using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct DealDamage : IComponentData
    {
        public int Value;
    }
}
