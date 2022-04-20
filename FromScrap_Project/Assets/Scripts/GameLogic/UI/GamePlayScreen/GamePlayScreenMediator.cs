using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using UniRx;
using UnityEngine;
using Zenject;

public class GamePlayScreenMediator : Mediator<GamePlayScreenView>
{
    [Inject]
    public void Init(ISignalService signalService)
    {
        signalService.Receive<OnExperienceChangeSignal>().Subscribe(OnExperienceGathered).AddTo(DisposeOnDestroy);
        
        //signalService.Receive<NewLevelSignal>().Subscribe().AddTo(DisposeOnDestroy);
    }

    private void OnExperienceGathered(OnExperienceChangeSignal signal)
    {
        View.SetExperience(signal.Experience/125.0f);
    }

    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();
        
    
    }

}
