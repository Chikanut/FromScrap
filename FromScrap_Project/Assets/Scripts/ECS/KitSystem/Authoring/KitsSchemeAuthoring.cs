using System.Collections.Generic;
using Cars.View.Components;
using Kits.Components;
using Packages.Common.Storage.Config.Cars;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace Kits.Authoring
{
    public class KitsSchemeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class KitPlatformData
        {
            public GameObject KitPlatform;
            public List<KitType> ConnectionTypes;
            public int[] DefaultKits;
            public bool CanOccupy;
        }
        
        [SerializeField] List<KitPlatformData> _kitPlatforms = new List<KitPlatformData>();


        private ICarsConfigController _carsConfigController;
        
        [Inject]
        public void Init(ICarsConfigController carsConfigController)
        {
            _carsConfigController = carsConfigController;
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            ProjectContext.Instance.Container.Inject(this);
            
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

                var addKitBuffer = dstManager.AddBuffer<AddKitBuffer>(entity);
                
                for (int i = 0; i < data.DefaultKits.Length; i++)
                {
                    addKitBuffer.Add(new AddKitBuffer()
                    {
                        PlatformID = i,
                        CarID = 0, //get car ID component
                        KitID = data.DefaultKits[i],
                        KitLevel = 0//add kit level settings
                    });
                }
            }
        }
    }
    
    // // var ID = dstManager.GetComponentData<CarIDComponent>(entity).ID;
    // var carData = _carsConfigController.GetCarData(0);
    // for (int i = 0; i < data.DefaultKits.Length; i++)
    // {
    //     var i1 = i;
    //     EntityPoolManager.Instance.GetObject(carData.UpgradesConfigs[data.DefaultKits[i]].KitObjects[0].gameObject, (kit, manager) =>
    //     {
    //         dstManager.AddComponentData(kit, new KitInstalatorTargetComponent() {TargetEntity = platformsBuffer[i1].Platform});
    //     });
    // }
}
