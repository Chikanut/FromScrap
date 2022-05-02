using BovineLabs.Event.Systems;
using SpawnGameObjects.Components;
using Unity.Transforms;
using UnityEngine;

public class SpawnEntityPoolObjectSystem : ConsumeSingleEventSystemBase<SpawnEntityPoolObjectEvent>
{
    protected override void OnEvent(SpawnEntityPoolObjectEvent e)
    {
        var chance = Random.Range(0, 100);
        
        if(chance > e.SpawnChance) return;
        
        EntityPoolManager.Instance.GetObject(e.EntityName.Value, (entity, manager) =>
        {
            manager.SetComponentData(entity, new Translation()
            {
                Value = e.Position
            });
        });
    }
}
