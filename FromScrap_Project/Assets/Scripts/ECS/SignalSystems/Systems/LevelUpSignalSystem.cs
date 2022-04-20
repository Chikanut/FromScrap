using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using Zenject;

public struct LevelUpSignal : ISignal
{
    public int Level;
}

namespace SignalSystems
{
    public class LevelUpSignalSystem : ConsumeSingleEventSystemBase<LevelUpSignal>
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

        protected override void OnEvent(LevelUpSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
}
