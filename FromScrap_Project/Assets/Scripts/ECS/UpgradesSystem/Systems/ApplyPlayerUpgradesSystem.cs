using System.Collections.Generic;
using Packages.Common.Storage.Config;
using ShootCommon.Signals;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UpgradesSystem.Components;
using Visartech.Progress;
using Zenject;

namespace UpgradesSystem.Systems
{
    public partial class ApplyPlayerUpgradesSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
        private IPlayerProgressionConfigController _playerProgressionConfigController;

        private readonly List<(Entity target, GameObject upgrade)> _spawnUpgrades =
            new List<(Entity target, GameObject upgrade)>();

        protected override void OnCreate()
        {
            base.OnCreate();

            _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            ProjectContext.Instance.Container.Inject(this);
        }
        
        [Inject]
        public void Init(ISignalService signalService,
            IPlayerProgressionConfigController playerProgressionConfigController)
        {
            _playerProgressionConfigController = playerProgressionConfigController;
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();

            Entities.ForEach((Entity entity, in ApplyUpgradesComponent applyUpgradesComponent) =>
            {
                for (int i = 0; i < Progress.Upgrades.Upgrades.Count; i++)
                {
                    var upgradeInfo = Progress.Upgrades.Upgrades[i];
                    if (upgradeInfo.Level == 0)
                        continue;

                    var upgradeConfig =
                        _playerProgressionConfigController.GetPlayerProgressionData.Upgrades.GetUpgradeLevelData(
                            upgradeInfo.CollectionID, upgradeInfo.UpgradeID, upgradeInfo.Level);

                    if (upgradeConfig == null || upgradeConfig.Authoring == null)
                        continue;

                    _spawnUpgrades.Add((entity, upgradeConfig.Authoring));
                }

                ecb.RemoveComponent<ApplyUpgradesComponent>(entity);

            }).WithoutBurst().Run();

            for (int i = 0; i < _spawnUpgrades.Count; i++)
            {
                var i1 = i;
                EntityPoolManager.Instance.GetObject(_spawnUpgrades[i].upgrade,
                    (entity, manager) =>
                    {
                        manager.AddComponentData(entity, new UpgradeComponent() {Target = _spawnUpgrades[i1].target});
                    });
            }

            _spawnUpgrades.Clear();
        }
    }
}
