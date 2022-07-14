using DG.Tweening;
using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Stateless;
using UI.Screens.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameLogic.States.States
{
    public class LoadGameSceneState : GlobalState
    {
        public const string GameScene = "GameScene";
        
        protected override void Configure()
        {
            Permit<LoadGameResourcesState>(StateMachineTriggers.LoadGameResources);
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
            SceneManager.LoadScene(GameScene, new LoadSceneParameters()
            {
                loadSceneMode = LoadSceneMode.Single,
                localPhysicsMode = LocalPhysicsMode.None
            });
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == GameScene)
                OnStartGame();
        }
        
        void OnStartGame()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Fire(StateMachineTriggers.LoadGameResources);
        }
    }
}