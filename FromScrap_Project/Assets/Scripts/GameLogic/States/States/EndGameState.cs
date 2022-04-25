using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using UI.Screens.Loading;
using Zenject;

namespace GameLogic.States.States
{
    public class EndGameState : GlobalState
    {
        protected override void Configure()
        {
            Permit<LoadMenuSceneState>(StateMachineTriggers.LoadMenuScene);
            Permit<LoadGameSceneState>(StateMachineTriggers.LoadGameScene);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            _menuNavigationController.HideAllMenuScreens();
            _menuNavigationController.ShowMenuScreen<EndGameScreenView>(SubscribeToSignals, "EndGameScreen");
        }
        
        private void SubscribeToSignals()
        {
            SubscribeToSignal<GoToMainMenuSignal>(GoToMainMenu);
            SubscribeToSignal<RestartGameSignal>(RestartGame);
        }
        
        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }
        
        void GoToMainMenu(GoToMainMenuSignal signal)
        {
            _menuNavigationController.HideAllMenuScreens();

            ECS_Logic_Extentions.ClearScene(() => { Fire(StateMachineTriggers.LoadMenuScene); });
        }

        void RestartGame(RestartGameSignal signal)
        {
            _menuNavigationController.HideAllMenuScreens();
            
            ECS_Logic_Extentions.ClearScene(() => { Fire(StateMachineTriggers.LoadGameScene); });
        }
    }
}