using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Hybrid.Internal;
using Unity.Mathematics;
using UnityEngine;
using WeaponsSystem.Base.Components;

public class WeaponAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    enum ShootType
    {
        Input,
        ShotIfRotated
    }
    
    [Header("Muzzle Info")] 
    [SerializeField] private float3 _shotOffset;
    [SerializeField] private float _shotFrequency;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private ShootType _shootType;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var muzzleData = new MuzzleData()
        {
            Offset = _shotOffset,
            ShotFrequency = _shotFrequency
        };
        var spawnShotData = new SpawnShotData();
        var shotPrefab = new ShotPrefab()
        {
            Value = conversionSystem.GetPrimaryEntity(_projectile)
        };

        dstManager.AddComponentData(entity, muzzleData);
        dstManager.AddComponentData(entity, spawnShotData);
        dstManager.AddComponentData(entity, shotPrefab);
        dstManager.AddComponentData(entity, new IsShotData());

        switch (_shootType)
        {
            case ShootType.Input:
                break;
            case ShootType.ShotIfRotated:
                dstManager.AddComponentData(entity, new ShotIfRotatedTag());
                break;
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        GeneratedAuthoringComponentImplementation
            .AddReferencedPrefab(referencedPrefabs, _projectile);
    }
}