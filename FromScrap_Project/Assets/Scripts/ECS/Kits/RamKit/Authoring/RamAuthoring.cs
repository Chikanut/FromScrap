using DamageSystem.Components;
using Ram.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using UnityEngine;

namespace Ram.Authorings
{
    public class RamAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private float2 _damageRange = new float2(5, 25);
        [SerializeField] private float2 _impulseRange = new float2(5, 25);
        [SerializeField] private float2 _speedRange = new float2(5, 25);
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new RamComponent()
            {
                DamageRange = _damageRange,
                ImpulseRange = _impulseRange,
                SpeedRange = _speedRange
            });
            dstManager.AddComponentData(entity, new DealDamage()
            {
                DamageDelay = 0.5f
            });

            dstManager.AddComponentData(entity, new CollisionEventBuffer() {CalculateDetails = 0});
            dstManager.AddBuffer<StatefulCollisionEvent>(entity);
        }
    }
}