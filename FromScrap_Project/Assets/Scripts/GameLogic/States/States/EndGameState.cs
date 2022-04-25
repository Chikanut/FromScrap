using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Stateless;
using Zenject;

namespace GameLogic.States.States
{
    public class EndGameState : GlobalState
    {
        protected override void Configure()
        {
            Permit<MainMenuState>(StateMachineTriggers.MainMenu);
            Permit<LoadGameSceneState>(StateMachineTriggers.LoadGameScene);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            //TODO: Add end game screen
            GoToMainMenu();
        }
        
        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }
        
        void GoToMainMenu()
        {
            _menuNavigationController.HideMenuScreen<GamePlayScreenView>(null, "GamePlayScreen");
            ECS_Logic_Extentions.ClearScene(() => { Fire(StateMachineTriggers.MainMenu); });
        }
    }
}