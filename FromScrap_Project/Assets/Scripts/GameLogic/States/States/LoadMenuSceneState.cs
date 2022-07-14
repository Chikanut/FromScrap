using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Stateless;
using UI.Screens.Loading;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameLogic.States.States
{
    public class LoadMenuSceneState : GlobalState
    {
        public const string MenuScene = "MainMenu";
        
        protected override void Configure()
        {
            Permit<MainMenuState>(StateMachineTriggers.MainMenu);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            _menuNavigationController.ShowMenuScreen<LoadingScreenView>(LoadScene, "LoadingScreen");
        }
        
        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }
        
        void LoadScene()
        {
            SceneManager.LoadScene(MenuScene, new LoadSceneParameters()
            {
                loadSceneMode = LoadSceneMode.Single,
                localPhysicsMode = LocalPhysicsMode.None
            });
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == MenuScene)
                OnStartGame();
        }
        
        void OnStartGame()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Fire(StateMachineTriggers.MainMenu);
        }
    }
}