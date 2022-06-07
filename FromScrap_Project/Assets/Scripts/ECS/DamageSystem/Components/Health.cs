using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DamageSystem.Components
{
    [Serializable]
    public struct Health : IComponentData
    {
        public bool ShowHitsNumbers;
        
        public int InitialMaxValue;
        public int CurrentMaxValue;
        public int Value;

        /// <summary>
        /// Dont get any damage after hit in seconds
        /// </summary>
        public float OnDamageBlockTime;

        public Health(int points, float onDamageBlockTime, bool showHitsNumbers)
        {
            InitialMaxValue = CurrentMaxValue = Value = points;
            OnDamageBlockTime = onDamageBlockTime;
            ShowHitsNumbers = showHitsNumbers;
        }

        public void SetMaxHealth(int maxHealth, bool rewriteInitialMax = false)
        {
            var healthToAdd = 0;
            
            if(rewriteInitialMax)
                InitialMaxValue = Value = maxHealth;
            else
                healthToAdd = maxHealth - CurrentMaxValue;

            CurrentMaxValue = maxHealth;
            
            if (healthToAdd > 0)
                AddHealth(healthToAdd);
        }

        public void AddHealth(int value)
        {
            Value = math.clamp(Value + value, 0, CurrentMaxValue);
        }
    }
}
