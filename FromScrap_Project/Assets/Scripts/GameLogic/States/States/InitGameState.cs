using ShootCommon.GlobalStateMachine;
using Stateless;
using UnityEngine;

namespace Packages.Common.StateMachineGlobal.States
{
    public class InitGameState : GlobalState
    {
        protected override void Configure()
        {

        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            
        }
    }
}