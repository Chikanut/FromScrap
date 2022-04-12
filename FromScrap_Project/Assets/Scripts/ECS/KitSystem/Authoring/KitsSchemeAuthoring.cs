using System.Collections.Generic;
using Kits.Components;
using Unity.Entities;
using UnityEngine;

namespace Kits.Authoring
{
    public class KitsSchemeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class KitPlatformData
        {
            public GameObject KitPlatform;
            public List<KitType> ConnectionTypes;
            public GameObject[] DefaultKits;
            public bool CanOccupy;

        }

        [SerializeField] List<KitPlatformData> _kitPlatforms = new List<KitPlatformData>();

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddBuffer<KitSchemeBuffer>(entity);

            foreach (var data in _kitPlatforms)
            {
                var platform = conversionSystem.GetPrimaryEntity(data.KitPlatform);

                var componentData = new KitPlatformComponent() {CanOccupy = data.CanOccupy, IsFree = true};

                dstManager.AddComponentData(platform, componentData);

                var connectionTypesBuffer = dstManager.AddBuffer<KitPlatformConnectionBuffer>(platform);
                foreach (var type in data.ConnectionTypes)
                    connectionTypesBuffer.Add(new KitPlatformConnectionBuffer() {ConnectionType = type});

                dstManager.AddBuffer<KitPlatformKitsBuffer>(platform);

                var platformsBuffer = dstManager.GetBuffer<KitSchemeBuffer>(entity);
                platformsBuffer.Add(new KitSchemeBuffer() {Platform = platform});

                for (int i = 0; i < data.DefaultKits.Length; i++)
                {
                    EntityPoolManager.Instance.GetObject(data.DefaultKits[i], (kit, manager) =>
                    {
                        dstManager.AddComponentData(kit,
                            new KitInstalatorTargetComponent() {TargetEntity = platform});
                    });
                }
            }
        }
    }
}
