using Kits.Components;
using Unity.Entities;
using UnityEngine;
using EditorTools;

namespace Kits.Authoring
{
    public class KitComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [HideInInspector] public int ID;
        [ReadOnly] public KitType Type;
        [ReadOnly] public bool IsStacked;
        [ReadOnly] public int KitLevel;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new KitComponent()
            {
                ID = ID,
                Type = Type,
                IsStacked = IsStacked,
                KitLevel = KitLevel
            });
        }
    }
}
