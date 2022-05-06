using System;
using Unity.Entities;
using UnityEngine;

namespace WeaponsSystem.Base.Components
{
    public enum MuzzleType
    {
        All,
        Queue
    }

    public enum WeaponState
    {
        None,
        NoTarget,
        Charge,
        Shoot,
        Reload,
    }

    [Serializable]
    public struct WeaponData : IComponentData
    {
        public MuzzleType MuzzleType;
        
        public float ChargeTime;
        public float ReloadTime;
        public float ShootTime;

        [HideInInspector] public WeaponState CurrentState;
        [HideInInspector] public WeaponState NewState;
        [HideInInspector] public int CurrentMuzzle;
        [HideInInspector] public double PrevStateChangeTime;
    }
}
