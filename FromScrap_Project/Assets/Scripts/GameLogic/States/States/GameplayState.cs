using ShootCommon.GlobalStateMachine;
using Stateless;

namespace GameLogic.States.States
{
    public class GameplayState : GlobalState
    {
        protected override void Configure()
        {
            Permit<EndGameState>(StateMachineTriggers.EndGame);
            //TODO: Add pause state
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignals();
        }
        
        private void SubscribeToSignals()
        {
           SubscribeToSignal<OnEndGameSignal>(OnEndGame);
        }

        void OnEndGame(OnEndGameSignal signal)
        {
            Fire(StateMachineTriggers.EndGame);
        }
    }
}