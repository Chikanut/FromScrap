using System;
using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct Health : IComponentData
    {
        public int Value;
    }
}
