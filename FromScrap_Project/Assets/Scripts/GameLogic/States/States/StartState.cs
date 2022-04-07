using Packages.Common.Storage.Config;
using ShootCommon.GlobalStateMachine;
using Stateless;

namespace Packages.Common.StateMachineGlobal.States
{
    public class StartState : GlobalState
    {
        protected override void Configure()
        {
            Permit<InitGameState>(StateMachineTriggers.InitGame);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignal<ConfigUpdatedSignal>((signal) => { Fire(StateMachineTriggers.InitGame); });
            
            SignalService.Publish(new LoadGameInfoSignal());
        }
    }
}