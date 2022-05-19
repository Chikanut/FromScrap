using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using Unity.Entities;
using Zenject;

namespace ECS.SignalSystems.Systems
{
    public struct KitInstalledSignal : ISignal
    {
        public Entity Car;
    }

    public class KitInstalledSignalSystem : ConsumeSingleEventSystemBase<KitInstalledSignal>
    {
        protected override void Create()
        {
            base.Create();

            ProjectContext.Instance.Container.Inject(this);
        }
        
        private ISignalService _signalService;

        [Inject]
        public void Init(ISignalService signalService)
        {
            _signalService = signalService;
        }

        protected override void OnEvent(KitInstalledSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
}