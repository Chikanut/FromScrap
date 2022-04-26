using MenuNavigation;
using Packages.Common.Storage.Config;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using UI.Screens.Preloader;
using UnityEngine;
using UnityEngine.SceneManagement;
using Visartech.Progress;
using Zenject;

namespace GameLogic.States.States
{
    public class LoadInfoState : GlobalState
    {
        public const string PreloaderScene = "Preloader";
        
        protected override void Configure()
        {
            Permit<LoadMenuSceneState>(StateMachineTriggers.LoadMenuScene);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            Application.targetFrameRate = 120;
            
            if (Progress.Development.isTesting)
            {
                Progress.Development.isTesting = false;
                return;
            }

            SubscribeToSignals();
            LoadScene();
        }


        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }

        void SubscribeToSignals()
        {
            SubscribeToSignal<GameInfoUpdatedSignal>((signal) =>
            {
                OnSettingsLoaded();
            });
        }

        void LoadScene()
        {
            if (SceneManager.GetActiveScene().name == PreloaderScene)
            {
                SetupScene();
            }
            else
            {
                SceneManager.LoadScene(PreloaderScene, new LoadSceneParameters()
                {
                    loadSceneMode = LoadSceneMode.Single,
                    localPhysicsMode = LocalPhysicsMode.None
                });
                
                SceneManager.sceneLoaded += OnPreloaderSceneLoaded;
            }
        }
         
        private void OnPreloaderSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == PreloaderScene)
            {
                SetupScene();
            }
        }

        void SetupScene()
        {
            _menuNavigationController.ShowMenuScreen<PreloaderScreenView>(() =>
            {
                SignalService.Publish(new LoadGameInfoSignal());
            }, "PreloaderScreen");
        }

        protected override void OnExit()
        {
            base.OnExit();
            _menuNavigationController.HideMenuScreen<PreloaderScreenView>(null, "PreloaderScreen");
            SceneManager.sceneLoaded -= OnPreloaderSceneLoaded;
        }

        void OnSettingsLoaded()
        {
            Fire(StateMachineTriggers.LoadMenuScene);
        }
    }
}