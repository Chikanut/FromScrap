using System.Collections.Generic;
using DamageSystem.Components;
using MyBox;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace DamageSystem.Authoring
{
    public class HealthSystemAuthoring : SpawnAfterDeathAuthoring
    {
        [Header("Health Info")]
        [SerializeField] private int _health;

        [SerializeField] private float _blockAllDamageOnStartSeconds;
        
        [Header("Health Bar Info")]
        public bool addHealthBar;
        public float3 _healthBarOffset;

        [Header("High Light")]
        public bool highLightOnDamage = true;
        public List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

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


            if (addHealthBar)
            {
                var healthBarData = new AddHealthBarData()
                {
                    Offset = _healthBarOffset,
                };

                dstManager.AddComponentData(entity, healthBarData);
            }

            if (highLightOnDamage && _meshRenderers.Count > 0)
            {
                var highLightsBuffer = dstManager.AddBuffer<DamageHighLightMeshBuffer>(entity);

                for (int i = 0; i < _meshRenderers.Count; i++)
                {
                    var meshEntity = conversionSystem.GetPrimaryEntity(_meshRenderers[i].gameObject);
                    highLightsBuffer.Add(new DamageHighLightMeshBuffer()
                        {Entity = meshEntity});

                    dstManager.AddComponentData(meshEntity,
                        new URPMaterialPropertyBaseColor {Value = new float4(1, 1, 1, 1)});
                }
            }
        }
    }
}
