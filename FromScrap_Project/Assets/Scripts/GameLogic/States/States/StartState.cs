using Packages.Common.Storage.Config;
using ShootCommon.GlobalStateMachine;
using Stateless;

namespace Packages.StateMachineGlobal.States
{
    public class StartState : GlobalState
    {
        protected override void Configure()
        {

        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SignalService.Publish(new LoadGameInfoSignal());
        }
    }
}