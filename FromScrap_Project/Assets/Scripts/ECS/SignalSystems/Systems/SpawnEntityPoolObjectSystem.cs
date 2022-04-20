using BovineLabs.Event.Systems;
using SpawnGameObjects.Components;
using Unity.Transforms;

public class SpawnEntityPoolObjectSystem : ConsumeSingleEventSystemBase<SpawnEntityPoolObjectEvent>
{
    protected override void OnEvent(SpawnEntityPoolObjectEvent e)
    {
        EntityPoolManager.Instance.GetObject(e.EntityName.Value, (entity, manager) =>
        {
            manager.SetComponentData(entity, new Translation()
            {
                Value = e.Position
            });
        });
    }
}
