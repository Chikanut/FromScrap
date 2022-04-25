using GameLogic.States.States;
using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using UI.MainMenu;
using UnityEngine.SceneManagement;
using Zenject;

namespace Packages.Common.StateMachineGlobal.States
{
    public class MainMenuState : GlobalState
    {
         public const string MenuScene = "MainMenu";
        
        protected override void Configure()
        {
            Permit<LoadGameSceneState>(StateMachineTriggers.LoadGameScene);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            SubscribeToSignals();
            LoadScene();
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
        
        private void LoadScene()
        {
            if (SceneManager.GetActiveScene().name == MenuScene)
            {
                SetupScene();
            }
            else
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(MenuScene, new LoadSceneParameters()
                {
                    loadSceneMode = LoadSceneMode.Single,
                    localPhysicsMode = LocalPhysicsMode.None
                });
            }
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == MenuScene)
            {
                SetupScene();
            }
        }
        
        void SetupScene()
        {
             _menuNavigationController.ShowMenuScreen<MainMenuScreenView>(null, "MainMenuScreen");
        }

        void OnStartGame()
        {
            Fire(StateMachineTriggers.LoadGameScene);
        }

        protected override void OnExit()
        {
            base.OnExit();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _menuNavigationController.HideMenuScreen<MainMenuScreenView>(null, "MainMenuScreen");
        }
    }
}