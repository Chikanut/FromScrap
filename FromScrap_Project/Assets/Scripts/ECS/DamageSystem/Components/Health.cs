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
        public bool ShowHitsNumbers;
        
        public int InitialValue;
        [HideInInspector] public int Value;

        /// <summary>
        /// Dont get any damage after hit in seconds
        /// </summary>
        public float OnDamageBlockTime;

        public void AddHealth(int value)
        {
            Value = math.clamp(Value + value, 0, InitialValue);
        }
    }
}
