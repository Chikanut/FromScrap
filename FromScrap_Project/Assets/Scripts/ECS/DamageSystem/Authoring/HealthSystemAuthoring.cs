using System.Collections.Generic;
using DamageSystem.Components;
using Unity.Entities;
using Unity.Entities.Hybrid.Internal;
using Unity.Mathematics;
using UnityEngine;


namespace DamageSystem.Authoring
{
    public class HealthSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Health Info")]
        [SerializeField] private int _health;

        [SerializeField] private float _blockAllDamageOnStartSeconds;
        
        [Header("Health Bar Info")]
        [SerializeField] private bool _addHealthBar;
        [SerializeField] private float3 _healthBarOffset;

        [Header("Death Info")]
        [SerializeField] private string[] SpawnEntitiesOnDeath;
        [SerializeField] private string[] SpawnPoolObjectsOnDeath;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var health = new Health()
            {
                InitialValue = _health,
                Value = _health
            };

            dstManager.AddComponentData(entity, health);
            dstManager.AddBuffer<Damage>(entity);

            if (SpawnEntitiesOnDeath != null)
            {
                if (SpawnEntitiesOnDeath.Length > 0)
                {
                    var spawnAfterDeathBuffer = dstManager.AddBuffer<SpawnEntityOnDeathBuffer>(entity);
                    for (int i = 0; i < SpawnEntitiesOnDeath.Length; i++)
                    {
                        spawnAfterDeathBuffer.Add(new SpawnEntityOnDeathBuffer()
                            {SpawnEntity = SpawnEntitiesOnDeath[i]});
                    }
                }
            }
            
            if (SpawnPoolObjectsOnDeath != null)
            {
                if (SpawnPoolObjectsOnDeath.Length > 0)
                {
                    var spawnAfterDeathBuffer = dstManager.AddBuffer<SpawnPoolObjectOnDeathBuffer>(entity);
                    for (int i = 0; i < SpawnPoolObjectsOnDeath.Length; i++)
                    {
                        spawnAfterDeathBuffer.Add(new SpawnPoolObjectOnDeathBuffer()
                            {SpawnObjectName = SpawnPoolObjectsOnDeath[i]});
                    }
                }
            }

            if (_blockAllDamageOnStartSeconds > 0)
            {
                dstManager.AddComponentData(entity, new DamageBlockTimer() {Value = _blockAllDamageOnStartSeconds});
            }
            
            
            if (!_addHealthBar) return;

            var healthBarData = new AddHealthBarData()
            {
                Offset = _healthBarOffset,
            };
            
            dstManager.AddComponentData(entity, healthBarData);
        }
    }
}
