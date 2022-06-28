using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using Zenject;

namespace ECS.SignalSystems.Systems
{
    public struct OnGameResourcesSpawnedSignal : ISignal
    {
        
    }
    
    public class OnGameResourcesSpawnedSignalSystem : ConsumeSingleEventSystemBase<OnGameResourcesSpawnedSignal>
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

        protected override void OnEvent(OnGameResourcesSpawnedSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
}