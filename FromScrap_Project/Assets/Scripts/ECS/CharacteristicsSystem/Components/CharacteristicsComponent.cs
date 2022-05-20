using System;
using Unity.Entities;
using UnityEngine;

namespace StatisticsSystem.Components
{
    [System.Serializable]
    public struct Characteristics : IComparable<Characteristics>
    {
        [Header("Health")]
        public int MaxHealth;
        public int AdditionalHealth;
        public float HealthMultiplier;
        public float HealthRestoreMultiplier;//TODO: Implement
        public float DamageResistMultiplier;

        [Header("Areas")]
        public float AreaMultiplier;

        [Header("Damage")]
        public float DamageMultiplier;
        public float ReloadSpeedMultiplier;
        public float ChargeSpeedMultiplier;
        public float ProjectileSpeedMultiplier;
        public float ProjectileSizeMultiplier;
        public float EffectDurationMultiplier;//TODO: Implement
        public int AdditionalProjectiles;
        public int AdditionalDamage;

        [Header("Accuracy")] 
        public float LuckMultiplier;//TODO: Implement
        public float GrowthMultiplier;//TODO: Implement
        public float GreedMultiplier;//TODO: Implement
        
        [Header("Physical")]
        public float MovementSpeedMultiplier;
        public float TorqueMultiplier;//TODO: Implement

        public Characteristics(float defaultMultiplier = 1f, int defaultAdditionalCount = 0)
        {
            MaxHealth = defaultAdditionalCount;
            AdditionalHealth = defaultAdditionalCount;
            HealthMultiplier = defaultMultiplier;
            HealthRestoreMultiplier = defaultMultiplier;
            DamageResistMultiplier = defaultMultiplier;
            AreaMultiplier = defaultMultiplier;
            DamageMultiplier = defaultMultiplier;
            ReloadSpeedMultiplier = defaultMultiplier;
            ChargeSpeedMultiplier = defaultMultiplier;
            AdditionalProjectiles = defaultAdditionalCount;
            ProjectileSpeedMultiplier = defaultMultiplier;
            MovementSpeedMultiplier = defaultMultiplier;
            AdditionalDamage = defaultAdditionalCount;
            ProjectileSizeMultiplier = defaultMultiplier;
            EffectDurationMultiplier = defaultMultiplier;
            LuckMultiplier = defaultMultiplier;
            GrowthMultiplier = defaultMultiplier;
            GreedMultiplier = defaultMultiplier;
            TorqueMultiplier = defaultMultiplier;
        }

        public void Add(Characteristics other)
        {
            MaxHealth += other.MaxHealth;
            AdditionalHealth += other.AdditionalHealth;
            HealthMultiplier += other.HealthMultiplier;
            HealthRestoreMultiplier += other.HealthRestoreMultiplier;
            DamageResistMultiplier += other.DamageResistMultiplier;
            AreaMultiplier += other.AreaMultiplier;
            DamageMultiplier += other.DamageMultiplier;
            ReloadSpeedMultiplier += other.ReloadSpeedMultiplier;
            ChargeSpeedMultiplier += other.ChargeSpeedMultiplier;
            ProjectileSpeedMultiplier += other.ProjectileSpeedMultiplier;
            MovementSpeedMultiplier += other.MovementSpeedMultiplier;
            ProjectileSizeMultiplier += other.ProjectileSizeMultiplier;
            
            AdditionalProjectiles += other.AdditionalProjectiles;
            AdditionalDamage += other.AdditionalDamage;
            
            EffectDurationMultiplier += other.EffectDurationMultiplier;
            LuckMultiplier += other.LuckMultiplier;
            GrowthMultiplier += other.GrowthMultiplier;
            GreedMultiplier += other.GreedMultiplier;
            TorqueMultiplier += other.TorqueMultiplier;
        }

        public void Multiply(Characteristics other)
        {
            MaxHealth += other.MaxHealth;
            AdditionalHealth += other.AdditionalHealth;
            HealthMultiplier *= other.HealthMultiplier;
            HealthRestoreMultiplier *= other.HealthRestoreMultiplier;
            DamageResistMultiplier *= other.DamageResistMultiplier;
            AreaMultiplier *= other.AreaMultiplier;
            DamageMultiplier *= other.DamageMultiplier;
            ReloadSpeedMultiplier *= other.ReloadSpeedMultiplier;
            ChargeSpeedMultiplier *= other.ChargeSpeedMultiplier;
            ProjectileSpeedMultiplier *= other.ProjectileSpeedMultiplier;
            MovementSpeedMultiplier *= other.MovementSpeedMultiplier;
            ProjectileSizeMultiplier *= other.ProjectileSizeMultiplier;
            
            AdditionalProjectiles += other.AdditionalProjectiles;
            AdditionalDamage += other.AdditionalDamage;
            
            EffectDurationMultiplier *= other.EffectDurationMultiplier;
            LuckMultiplier *= other.LuckMultiplier;
            GrowthMultiplier *= other.GrowthMultiplier;
            GreedMultiplier *= other.GreedMultiplier;
            TorqueMultiplier *= other.TorqueMultiplier;
        }

        public int CompareTo(Characteristics other)
        {
            var healthMultiplierComparison = HealthMultiplier.CompareTo(other.HealthMultiplier);
            if (healthMultiplierComparison != 0) return healthMultiplierComparison;
            var healthRestoreMultiplierComparison = HealthRestoreMultiplier.CompareTo(other.HealthRestoreMultiplier);
            if (healthRestoreMultiplierComparison != 0) return healthRestoreMultiplierComparison;
            var damageResistMultiplierComparison = DamageResistMultiplier.CompareTo(other.DamageResistMultiplier);
            if (damageResistMultiplierComparison != 0) return damageResistMultiplierComparison;
            var areaMultiplierComparison = AreaMultiplier.CompareTo(other.AreaMultiplier);
            if (areaMultiplierComparison != 0) return areaMultiplierComparison;
            var damageMultiplierComparison = DamageMultiplier.CompareTo(other.DamageMultiplier);
            if (damageMultiplierComparison != 0) return damageMultiplierComparison;
            var reloadSpeedMultiplierComparison = ReloadSpeedMultiplier.CompareTo(other.ReloadSpeedMultiplier);
            if (reloadSpeedMultiplierComparison != 0) return reloadSpeedMultiplierComparison;
            var chargeSpeedMultiplierComparison = ChargeSpeedMultiplier.CompareTo(other.ChargeSpeedMultiplier);
            if (chargeSpeedMultiplierComparison != 0) return chargeSpeedMultiplierComparison;
            var additionalProjectilesComparison = AdditionalProjectiles.CompareTo(other.AdditionalProjectiles);
            if (additionalProjectilesComparison != 0) return additionalProjectilesComparison;
            var projectileSpeedMultiplierComparison = ProjectileSpeedMultiplier.CompareTo(other.ProjectileSpeedMultiplier);
            if (projectileSpeedMultiplierComparison != 0) return projectileSpeedMultiplierComparison;
            var movementSpeedComparison = MovementSpeedMultiplier.CompareTo(other.MovementSpeedMultiplier);
            if (movementSpeedComparison != 0) return movementSpeedComparison;
            var projectileSizeComparison = ProjectileSizeMultiplier.CompareTo(other.ProjectileSizeMultiplier);
            if (projectileSizeComparison != 0) return projectileSizeComparison;
            var effectDurationComparison = EffectDurationMultiplier.CompareTo(other.EffectDurationMultiplier);
            if (effectDurationComparison != 0) return effectDurationComparison;
            var luckComparison = LuckMultiplier.CompareTo(other.LuckMultiplier);
            if (luckComparison != 0) return luckComparison;
            var growthComparison = GrowthMultiplier.CompareTo(other.GrowthMultiplier);
            if (growthComparison != 0) return growthComparison;
            var greedComparison = GreedMultiplier.CompareTo(other.GreedMultiplier);
            if (greedComparison != 0) return greedComparison;
            var torqueComparison = TorqueMultiplier.CompareTo(other.TorqueMultiplier);
            if (torqueComparison != 0) return torqueComparison;
            var maxHealthComparison = MaxHealth.CompareTo(other.MaxHealth);
            return maxHealthComparison;
        }
    }

    public struct CharacteristicsComponent : IComponentData, IComparable<Characteristics>
    {
        public Characteristics Value;

        public void Add(Characteristics other)
        {
            Value.Add(other);
        }
        
        public void Multiply(Characteristics other)
        {
            Value.Multiply(other);
        }


        public int CompareTo(Characteristics other)
        {
            return Value.CompareTo(other);
        }
    }
}
