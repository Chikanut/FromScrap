using System.Collections.Generic;
using System.Threading.Tasks;
using Kits.Components;
using LevelingSystem.Components;
using MenuNavigation;
using ShootCommon.Signals;
using UI.Upgrades;
using Unity.Entities;
using Zenject;

namespace Upgrades.Systems
{
    public partial class UpgradesSystem : SystemBase
    {

        public List<(Entity upgradeObject, int upgradeLevel)> _upgradesQueue = new List<(Entity, int)>();
        private IMenuNavigationController _menuNavigationController;

        private bool _isUpgrading;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            ProjectContext.Instance.Container.Inject(this);
        }
    
        [Inject]
        public void Init(ISignalService signalService, IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }

        private bool isShowingUpgrade = false;
        
        protected override void OnUpdate()
        {
            if(isShowingUpgrade) return;
            Entities.WithAll<KitSchemeBuffer>().ForEach((Entity entity, ref DynamicBuffer<NewLevelBuffer> levelBuffers) =>
            {
                for (int i = 0; i < levelBuffers.Length; i++)
                    _upgradesQueue.Add((entity, levelBuffers[i].Level));

                levelBuffers.Clear();
            }).WithoutBurst().Run();

            if (_isUpgrading || _upgradesQueue.Count <= 0) return;
            
            Upgrade(_upgradesQueue[0].upgradeObject, _upgradesQueue[0].upgradeLevel);
            _upgradesQueue.RemoveAt(0);

        }

        async Task Upgrade(Entity upgradeEntity, int level)
        {
            UnityEngine.Time.timeScale = 0;
            _isUpgrading = true;
            
           var menuScreen = await _menuNavigationController.ShowMenuScreen<UpgradeScreenView>(null,"UpgradesScreen");
           menuScreen.Init(upgradeEntity, level, CompleteUpgrade);
        }

        void CompleteUpgrade()
        {
            UnityEngine.Time.timeScale = 1;
            _isUpgrading = false;

        }
    }
}