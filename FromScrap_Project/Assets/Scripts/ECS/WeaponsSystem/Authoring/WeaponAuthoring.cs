using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using WeaponsSystem.Base.Components;
using Gizmos = Popcron.Gizmos;

namespace WeaponsSystem.Base.Authoring
{
    public class WeaponAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        enum ShootType
        {
            Input,
            ShotIfRotated
        }
        
        [Serializable]
        public class MuzzlesInfo
        {
            public string Projectile;

            public float3 Offset;
            public float3 Direction = new float3(0, 0, 1);
            [Range(0,90)]
            public float ShootSpray;
            
            public int ShotsCount = 1;
            [Range(0,360)]
            public float ShotsAngle = 0;
            public float3 ShotsAngleAxis = new float3(0, 1, 0);

            public int ShotAnimationIndex = -1;
        }
        
        [Header("Weapon Info")]
        [SerializeField] ShootType _shootEventType;
        [SerializeField] MuzzleType _shootType;
        [SerializeField] GameObject _weaponView;
        [SerializeField] float _shotFrequency;

        [Header("Muzzle Info")] [SerializeField]
        private MuzzlesInfo[] _muzzlesInfo;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            
            dstManager.AddComponentData(entity, new WeaponData()
            {
                WeaponView = conversionSystem.GetPrimaryEntity(_weaponView),
                MuzzleType = _shootType,
                ShotFrequency = _shotFrequency
            });
            dstManager.AddComponentData(entity, new IsShotData());
            var muzzleBuffer = dstManager.AddBuffer<MuzzlesBuffer>(entity);

            for (int i = 0; i < _muzzlesInfo.Length; i++)
            {
                muzzleBuffer.Add(new MuzzlesBuffer()
                {
                    Offset = _muzzlesInfo[i].Offset,
                    Direction = _muzzlesInfo[i].Direction,
                    Projectile = _muzzlesInfo[i].Projectile,
                    ShootSpray = _muzzlesInfo[i].ShootSpray,
                    ShotAnimationIndex = _muzzlesInfo[i].ShotAnimationIndex,
                    ShotsCount = _muzzlesInfo[i].ShotsCount,
                    ShotsAngle = _muzzlesInfo[i].ShotsAngle,
                    ShotsAngleAxis = _muzzlesInfo[i].ShotsAngleAxis
                });
            }

            switch (_shootEventType)
            {
                case ShootType.Input:
                    break;
                case ShootType.ShotIfRotated:
                    dstManager.AddComponentData(entity, new ShotIfRotatedTag());
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < _muzzlesInfo.Length; i++)
            {
                var angleStep = _muzzlesInfo[i].ShotsAngle / (_muzzlesInfo[i].ShotsCount);
                var startAngle = -(_muzzlesInfo[i].ShotsAngle / 2f) - (angleStep/2f);
                for (int j = 0; j < _muzzlesInfo[i].ShotsCount; j++)
                {
                    var angle = startAngle + angleStep * (j+1);
                    var direction = Quaternion.AngleAxis(angle, _muzzlesInfo[i].ShotsAngleAxis) * _muzzlesInfo[i].Direction;
                    
                    Gizmos.Cone(transform.TransformPoint(_muzzlesInfo[i].Offset),
                        Quaternion.LookRotation(transform.TransformDirection(direction)), 0.4f,
                        _muzzlesInfo[i].ShootSpray, Color.green);
                }
            }
        }
    }
}