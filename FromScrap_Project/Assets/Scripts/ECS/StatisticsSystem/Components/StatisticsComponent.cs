using System;
using Unity.Entities;
using UnityEngine;

namespace StatisticsSystem.Components
{
    public struct Statistics : IComparable<Statistics>
    {
        [Header("Health")]
        public float HealthMultiplier;
        public float HealthRestoreMultiplier;
        public float DamageResistMultiplier;

        [Header("Areas")]
        public float AreaMultiplier;

        [Header("Damage")]
        public float DamageMultiplier;
        public float ReloadSpeedMultiplier;
        public float ChargeSpeedMultiplier;
        public int AdditionalProjectiles;
        public float ProjectileSpeedMultiplier;

        [Header("Physical")]
        public float MovementSpeedMultiplier;
        
        public void Add(Statistics other)
        {
            HealthMultiplier += other.HealthMultiplier;
            HealthRestoreMultiplier += other.HealthRestoreMultiplier;
            DamageResistMultiplier += other.DamageResistMultiplier;
            AreaMultiplier += other.AreaMultiplier;
            DamageMultiplier += other.DamageMultiplier;
            ReloadSpeedMultiplier += other.ReloadSpeedMultiplier;
            ChargeSpeedMultiplier += other.ChargeSpeedMultiplier;
            AdditionalProjectiles += other.AdditionalProjectiles;
            ProjectileSpeedMultiplier += other.ProjectileSpeedMultiplier;
            MovementSpeedMultiplier += other.MovementSpeedMultiplier;
        }

        public int CompareTo(Statistics other)
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

    public struct StatisticsComponent : IComponentData, IComparable<Statistics>
    {
        public Statistics Statistics;

        public void Add(Statistics other)
        {
            Statistics.Add(other);
        }


        public int CompareTo(Statistics other)
        {
            return Statistics.CompareTo(other);
        }
    }
}
