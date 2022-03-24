using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    [Serializable]
    public class HealthBarUI : IComponentData
    {
        public Transform SliderContainer;
        public Slider Slider;
        public float3 Offset;
    }
}
