using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;

namespace GameLogic.States.States
{
    public class GameplayState : GlobalState
    {
        protected override void Configure()
        {
            Permit<EndGameState>(StateMachineTriggers.EndGame);
            Permit<PauseGameState>(StateMachineTriggers.Pause);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignals();
        }
        
        private void SubscribeToSignals()
        {
           SubscribeToSignal<OnEndGameSignal>(OnEndGame);
           SubscribeToSignal<OnPauseSignal>(OnPause);
        }

        private void OnPause(OnPauseSignal signal)
        {
            Fire(StateMachineTriggers.Pause);
        }

        void OnEndGame(OnEndGameSignal signal)
        {
            Fire(StateMachineTriggers.EndGame);
        }
    }
}