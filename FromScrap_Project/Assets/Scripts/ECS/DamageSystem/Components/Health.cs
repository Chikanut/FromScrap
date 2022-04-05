using System;
using Unity.Entities;
using UnityEngine;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public struct Health : IComponentData
    {
        public int InitialValue;
        [HideInInspector] public int Value;
    }
}
