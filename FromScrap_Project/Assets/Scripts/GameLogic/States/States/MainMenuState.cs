using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using UI.Screens.Loading;
using UI.Screens.MainMenu;
using Zenject;

namespace GameLogic.States.States
{
    public class MainMenuState : GlobalState
    {
        protected override void Configure()
        {
            Permit<LoadGameSceneState>(StateMachineTriggers.LoadGameScene);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            _menuNavigationController.HideMenuScreen<LoadingScreenView>(SetupScene, "LoadingScreen");
        }
        
        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }
        
        private void SubscribeToSignals()
        {
            SubscribeToSignal<StartGameSignal>((signal) =>
            {
                OnStartGame();
            });
        }
        
        void SetupScene()
        {
            SubscribeToSignals();
            _menuNavigationController.ShowMenuScreen<MainMenuScreenView>(null, "MainMenuScreen");
        }

        void OnStartGame()
        {
            Fire(StateMachineTriggers.LoadGameScene);
        }

        protected override void OnExit()
        {
            base.OnExit();
            _menuNavigationController.HideMenuScreen<MainMenuScreenView>(null, "MainMenuScreen");
        }
    }
}