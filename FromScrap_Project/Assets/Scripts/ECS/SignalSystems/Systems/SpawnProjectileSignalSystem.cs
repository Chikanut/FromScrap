using BovineLabs.Event.Systems;
using DamageSystem.Components;
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
    public float DamageMultiplier;
    public float AreaMultiplier;
    public float SpeedMultiplier;
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
                CurrentLife = 0,
                InitialPosition = signal.SpawnPos,
                MoveDir = signal.SpawnDir,
                SpeedMultiplier = signal.SpeedMultiplier
            });

            if (manager.HasComponent<DealDamage>(entity))
            {
                var damageInfo = manager.GetComponentData<DealDamage>(entity);
                damageInfo.Value = (int)(damageInfo.Value * signal.DamageMultiplier);
                manager.SetComponentData(entity, damageInfo);
            }

            if (manager.HasComponent<SphereTriggerComponent>(entity))
            {
                var triggerInfo = manager.GetComponentData<SphereTriggerComponent>(entity);
                triggerInfo.Radius *= signal.AreaMultiplier;
                manager.SetComponentData(entity, triggerInfo);
                
                if (manager.HasComponent<Scale>(entity))
                {
                    var scale = manager.GetComponentData<Scale>(entity);
                    scale.Value *= signal.AreaMultiplier;
                
                    manager.SetComponentData(entity, scale);
                }
                else
                {
                    manager.AddComponentData(entity, new Scale()
                    {
                        Value = 1 * signal.AreaMultiplier
                    });
                }
            }
        }));
    }
}

