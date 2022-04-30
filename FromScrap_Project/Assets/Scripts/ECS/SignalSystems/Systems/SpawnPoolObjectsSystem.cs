using BovineLabs.Event.Systems;
using SpawnGameObjects.Components;
using UnityEngine;

namespace SpawnGameObjects.Systems
{
    public partial class SpawnPoolObjectsSystem : ConsumeSingleEventSystemBase<SpawnPoolObjectEvent>
    {
        protected override void OnEvent(SpawnPoolObjectEvent e)
        {
            var chance = Random.Range(0, 100);
        
            if(chance > e.SpawnChance) return;
            
            var instance = ObjectsPool.Instance.GetObjectOfType<PoolObject>(e.SpawnObjectName.Value);
            instance.transform.position = e.Position;
        }
    }
}