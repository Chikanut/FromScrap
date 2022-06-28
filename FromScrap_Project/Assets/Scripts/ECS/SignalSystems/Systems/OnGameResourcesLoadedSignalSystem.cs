using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using Zenject;

namespace ECS.SignalSystems.Systems
{
    public struct OnGameResourcesLoadedSignal : ISignal
    {
        
    }
    
    public class OnGameResourcesLoadedSignalSystem : ConsumeSingleEventSystemBase<OnGameResourcesLoadedSignal>
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

        protected override void OnEvent(OnGameResourcesLoadedSignal signal)
        {
           _signalService.Publish(signal);
        }
    }
}
