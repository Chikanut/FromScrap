using BovineLabs.Event.Systems;
using Unity.Collections;
using Unity.Mathematics;
using WeaponsSystem.Base.Components;

public struct SpawnProjectileEvent
{
    public FixedString32Bytes SpawnProjectileName;
    public float3 SpawnPos;
    public float3 SpawnDir;
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
                MoveDir = signal.SpawnDir
            });
        }));
    }
}

