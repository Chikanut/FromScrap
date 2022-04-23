using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct Health : IComponentData
    {
        public int InitialValue;
        [HideInInspector] public int Value;

        public void AddHealth(int value)
        {
            Value = math.clamp(Value + value, 0, InitialValue);
        }
    }
}
