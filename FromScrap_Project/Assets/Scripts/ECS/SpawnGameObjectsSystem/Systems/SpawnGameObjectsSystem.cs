using BovineLabs.Event.Systems;
using SpawnGameObjects.Components;

namespace SpawnGameObjects.Systems
{
    public partial class SpawnGameObjectsSystem : ConsumeSingleEventSystemBase<SpawnGameObjectEvent>
    {
        protected override void OnEvent(SpawnGameObjectEvent e)
        {
            var instance = ObjectsPool.Instance.GetObjectOfType<PoolObject>(e.SpawnObjectName.Value);
            instance.transform.position = e.Position;
        }
    }
}