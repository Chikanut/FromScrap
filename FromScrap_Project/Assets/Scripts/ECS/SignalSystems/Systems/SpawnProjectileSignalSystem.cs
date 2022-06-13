using BovineLabs.Event.Systems;
using DamageSystem.Components;
using StatisticsSystem.Components;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using WeaponsSystem.Base.Components;

public struct SpawnProjectileEvent
{
    public FixedString32Bytes SpawnProjectileName;
    public float3 SpawnPos;
    public float3 SpawnDir;
    public Characteristics Characteristics;
}

public class SpawnProjectileSystem : ConsumeSingleEventSystemBase<SpawnProjectileEvent>
{
    protected override void OnEvent(SpawnProjectileEvent signal)
    {
        EntityPoolManager.Instance.GetObject(signal.SpawnProjectileName.Value, ((entity, manager) =>
        {
            manager.AddComponentData(entity, new ShotTemporaryData()
            {
                MoveShot = true,
                InitialPosition = signal.SpawnPos,
                Direction = signal.SpawnDir,
                CurrentDirection = signal.SpawnDir,
                Characteristics = signal.Characteristics
            });

            manager.AddComponentData(entity, new Translation() {Value = signal.SpawnPos});

            if (manager.HasComponent<DealDamage>(entity))
            {
                var damageInfo = manager.GetComponentData<DealDamage>(entity);
                damageInfo.Value = (int)(damageInfo.Value * signal.Characteristics.DamageMultiplier);
                damageInfo.Value += signal.Characteristics.AdditionalDamage;
                manager.SetComponentData(entity, damageInfo);
            }

            if (manager.HasComponent<SphereTriggerComponent>(entity))
            {
                var triggerInfo = manager.GetComponentData<SphereTriggerComponent>(entity);
                triggerInfo.Radius *= signal.Characteristics.ProjectileSizeMultiplier;
                manager.SetComponentData(entity, triggerInfo);
                
                if (manager.HasComponent<Scale>(entity))
                {
                    var scale = manager.GetComponentData<Scale>(entity);
                    scale.Value *= signal.Characteristics.ProjectileSizeMultiplier;
                
                    manager.SetComponentData(entity, scale);
                }
                else
                {
                    manager.AddComponentData(entity, new Scale()
                    {
                        Value = 1 * signal.Characteristics.ProjectileSizeMultiplier
                    });
                }
            }
        }));
    }
}

