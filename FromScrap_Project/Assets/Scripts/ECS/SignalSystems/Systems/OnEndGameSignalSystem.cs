using BovineLabs.Event.Systems;
using DamageSystem.Components;
using DamageSystem.Systems;
using ShootCommon.Signals;
using Unity.Entities;
using Zenject;

public struct OnEndGameSignal : ISignal
{
        
}

namespace ECS.SignalSystems.Systems
{
    [UpdateAfter(typeof(ResolveDamageSystem))]
    public partial class OnEndGameCheckSystem : SystemBase
    {
        private EventSystem _eventSystem;
        protected override void OnCreate()
        {
            _eventSystem = this.World.GetOrCreateSystem<EventSystem>();
            base.OnCreate();
        }
        
        protected override void OnUpdate()
        {
            var endGameEventWriter = _eventSystem.CreateEventWriter<OnEndGameSignal>();
            Entities.WithAll<Dead>().WithAll<PlayerTag>().ForEach(() =>
            {
                endGameEventWriter.Write(new OnEndGameSignal());
            }).ScheduleParallel();
            
            _eventSystem.AddJobHandleForProducer<OnEndGameSignal>(Dependency);
        }
    }

    public class OnEndGameSignalSystem : ConsumeSingleEventSystemBase<OnEndGameSignal>
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

        protected override void OnEvent(OnEndGameSignal signal)
        {
            _signalService.Publish(signal);
        }
    }
}