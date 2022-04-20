using BovineLabs.Event.Systems;
using ShootCommon.Signals;
using Zenject;

public struct OnExperienceChangeSignal : ISignal
{
    public int Experience;
}

namespace SignalSystems
{
    public class ExperienceGatherSignalSystem : ConsumeSingleEventSystemBase<OnExperienceChangeSignal>
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

        protected override void OnEvent(OnExperienceChangeSignal e)
        {
            _signalService.Publish(e);
        }
    }
}
