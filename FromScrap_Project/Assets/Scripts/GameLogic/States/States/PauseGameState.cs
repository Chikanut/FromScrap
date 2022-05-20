using MenuNavigation;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using UI.PopUps.Pause;
using UnityEngine;
using Zenject;

namespace GameLogic.States.States
{
    public class PauseGameState : GlobalState
    {
        protected override void Configure()
        {
            Permit<GameplayState>(StateMachineTriggers.Game);
            Permit<LoadMenuSceneState>(StateMachineTriggers.LoadMenuScene);
            Permit<LoadGameSceneState>(StateMachineTriggers.LoadGameScene);
        }
        
        private IMenuNavigationController _menuNavigationController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController)
        {
            _menuNavigationController = menuNavigationController;
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            _menuNavigationController.ShowPopup<PausePopUpView>(SubscribeToSignals, "PausePopUp");
        }
        
        private void SubscribeToSignals()
        {
            SubscribeToSignal<ContinueGameSignal>(OnContinue);
            SubscribeToSignal<GoToMainMenuSignal>(GoToMainMenu);
            SubscribeToSignal<RestartGameSignal>(RestartGame);
        }

        void OnContinue(ContinueGameSignal signal)
        {
            _menuNavigationController.HidePopup<PausePopUpView>(() =>
            {
                Fire(StateMachineTriggers.Game);
            }, "PausePopUp");
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