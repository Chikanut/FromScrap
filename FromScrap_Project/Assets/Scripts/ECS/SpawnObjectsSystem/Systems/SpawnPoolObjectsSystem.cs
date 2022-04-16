using BovineLabs.Event.Systems;
using SpawnGameObjects.Components;

namespace SpawnGameObjects.Systems
{
    public partial class SpawnPoolObjectsSystem : ConsumeSingleEventSystemBase<SpawnPoolObjectEvent>
    {
        protected override void OnEvent(SpawnPoolObjectEvent e)
        {
            var instance = ObjectsPool.Instance.GetObjectOfType<PoolObject>(e.SpawnObjectName.Value);
            instance.transform.position = e.Position;
        }
    }
}