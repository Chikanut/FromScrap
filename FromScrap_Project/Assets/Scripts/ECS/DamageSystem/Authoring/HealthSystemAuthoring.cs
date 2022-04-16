using DamageSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DamageSystem.Authoring
{
    public class HealthSystemAuthoring : SpawnAfterDeathAuthoring
    {
        [Header("Health Info")]
        [SerializeField] private int _health;

        [SerializeField] private float _blockAllDamageOnStartSeconds;
        
        [Header("Health Bar Info")]
        [SerializeField] private bool _addHealthBar;
        [SerializeField] private float3 _healthBarOffset;

        public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            base.ConvertAncestors(entity, dstManager, conversionSystem);
            
            var health = new Health()
            {
                InitialValue = _health,
                Value = _health
            };

            dstManager.AddComponentData(entity, health);
            dstManager.AddBuffer<Damage>(entity);
            
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
