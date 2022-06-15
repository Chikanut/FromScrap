using System.Collections.Generic;
using Kits.Components;
using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.Upgrades;
using ShootCommon.Signals;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace Kits.Systems
{
    public partial class KitSpawnSystem : SystemBase
    {
        private ICarsConfigController _carsConfigController;
        private IUpgradesConfigController _upgradesConfigController;
        
        private readonly List<(Entity platform, int kitID, GameObject kit)> _spawnKits = new List<(Entity platform, int kitID, GameObject kit)>();

        protected override void OnCreate()
        {
            base.OnCreate();

            ProjectContext.Instance.Container.Inject(this);
        }
        
        [Inject]
        public void Init(ISignalService signalService, ICarsConfigController carsConfigController, IUpgradesConfigController upgradesConfigController)
        {
            _carsConfigController = carsConfigController;
            _upgradesConfigController = upgradesConfigController;
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref DynamicBuffer<KitAddBuffer> addKits, in DynamicBuffer<KitSchemeBuffer> scheme) =>
            {
                for (int i = 0; i < addKits.Length; i++)
                {
                    var kitInfo = _upgradesConfigController.GetUpgradesData.Kits[addKits[i].KitIndex].Data
                        .KitObjects[addKits[i].KitLevel];

                    var platform = scheme[addKits[i].PlatformID].Platform;
                    
                    _spawnKits.Add((platform, addKits[i].KitIndex, kitInfo.Authoring.gameObject));
                }
                
                addKits.Clear();
            }).WithoutBurst().Run();

            for (int i = 0; i < _spawnKits.Count; i++)
            {
                var i1 = i;
                EntityPoolManager.Instance.GetObject(_spawnKits[i].kit, (entity, manager) =>
                {
                    manager.AddComponentData(entity, new KitInstalatorTargetComponent() {TargetEntity = _spawnKits[i1].platform});
                    manager.AddComponentData(entity, new KitIDComponent() {Index = _spawnKits[i1].kitID});
                });
            }
            
            _spawnKits.Clear();
        }
    }
}
