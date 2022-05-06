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

    [Serializable]
    public struct WeaponData : IComponentData
    {
        public Entity WeaponView;
        
        public MuzzleType MuzzleType;
        public float ShotFrequency;
        
        [HideInInspector] public int CurrentMuzzle;
        [HideInInspector] public double PrevShotTime;
    }
}
