using ShootCommon.GlobalStateMachine;
using Stateless;

namespace Packages.Common.StateMachineGlobal.States
{
    public class InitState : GlobalState
    {
        protected override void Configure()
        {
             Permit<LoadInfoState>(StateMachineTriggers.LoadInfo);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {

        }
    }
}