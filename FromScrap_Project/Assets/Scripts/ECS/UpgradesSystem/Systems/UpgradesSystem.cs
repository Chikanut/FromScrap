using Kits.Components;
using LevelingSystem.Components;
using ShootCommon.Signals;
using Unity.Entities;
using Zenject;

namespace Upgrades.Systems
{
    public partial class UpgradesSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            ProjectContext.Instance.Container.Inject(this);
        }
    
        [Inject]
        public void Init(ISignalService signalService)
        {

        }

        private bool isShowingUpgrade = false;
        
        protected override void OnUpdate()
        {
            if(isShowingUpgrade) return;
            Entities.ForEach((Entity entity, ref DynamicBuffer<NewLevelBuffer> levelBuffers,
                in DynamicBuffer<KitSchemeBuffer> kitsScheme) =>
            {
                for (int i = 0; i < levelBuffers.Length; i++)
                    Upgrade(entity, kitsScheme, levelBuffers[i].Level);

                levelBuffers.Clear();
            }).WithoutBurst().Run();
        }

        void Upgrade(Entity upgradeEntity, DynamicBuffer<KitSchemeBuffer> kitsScheme, int level)
        {
            // UnityEngine.Time.timeScale = 0;
        }
    }
}