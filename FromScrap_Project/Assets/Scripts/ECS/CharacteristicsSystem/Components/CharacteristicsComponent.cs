using System;
using Unity.Entities;
using UnityEngine;

namespace StatisticsSystem.Components
{
    [System.Serializable]
    public struct Characteristics : IComparable<Characteristics>
    {
        [Header("Health")]
        public int AdditionalHealth;
        public float HealthMultiplier;
        public float HealthRestoreMultiplier;//not ready yet
        public float DamageResistMultiplier;

        [Header("Areas")]
        public float AreaMultiplier;

        [Header("Damage")]
        public float DamageMultiplier;
        public float ReloadSpeedMultiplier;
        public float ChargeSpeedMultiplier;
        public float ProjectileSpeedMultiplier;
        public float ProjectileSizeMultiplier;
        
        public int AdditionalProjectiles;
        public int AdditionalDamage;

        [Header("Physical")]
        public float MovementSpeedMultiplier;

        public Characteristics(float defaultMultiplier = 1f, int defaultAdditionalCount = 0)
        {
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
        }

        public void Add(Characteristics other)
        {
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
        }

        public void Multiply(Characteristics other)
        {
            AdditionalHealth *= other.AdditionalHealth;
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
            return MovementSpeedMultiplier.CompareTo(other.MovementSpeedMultiplier);
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
