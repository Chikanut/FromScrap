using System;
using MenuNavigation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopUps.Pause
{
    public class PausePopUpView : Popup
    {
        [SerializeField] private Button _continue;
        [SerializeField] private Button _mainMenu;
        [SerializeField] private Button _restart;

        public Action OnContinueAction;
        public Action OnMainMenuAction;
        public Action OnRestartAction;

        protected override void Start()
        {
            base.Start();
            
            _continue.onClick.AddListener(OnContinue);
            _mainMenu.onClick.AddListener(OnStartGame);
            _restart.onClick.AddListener(OnExitGame);
        }

        void OnContinue()
        {
            OnContinueAction?.Invoke();
        }

        void OnStartGame()
        {
            OnMainMenuAction?.Invoke();
        }

        void OnExitGame()
        {
            OnRestartAction?.Invoke();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Time.timeScale = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Time.timeScale = 1;
        }
    }
}