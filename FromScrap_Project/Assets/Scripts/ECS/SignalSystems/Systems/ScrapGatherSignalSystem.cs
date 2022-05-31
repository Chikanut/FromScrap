using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using Zenject;

public struct ScrapGatherSignal : ISignal
{
    public int Value;
}

namespace SignalSystems
{
    public class ScrapGatherSignalSystem : ConsumeSingleEventSystemBase<ScrapGatherSignal>
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

        protected override void OnEvent(ScrapGatherSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
}
