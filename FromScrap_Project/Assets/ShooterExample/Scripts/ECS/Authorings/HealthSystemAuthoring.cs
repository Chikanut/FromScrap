using DamageSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HealthSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Muzzle Info")] 
    [SerializeField] private int _health;
    [SerializeField] private bool _addHealthBar;
    [SerializeField] private float3 _healthBarOffset;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var health = new Health()
        {
            InitialValue = _health,
            Value = _health
        };
        
        dstManager.AddComponentData(entity, health);
        dstManager.AddBuffer<Damage>(entity);

        if (!_addHealthBar) return;
        
        var healthBarData = new AddHealthBarData()
        {
            Offset = _healthBarOffset,
        };

        dstManager.AddComponentData(entity, healthBarData);
    }
}
