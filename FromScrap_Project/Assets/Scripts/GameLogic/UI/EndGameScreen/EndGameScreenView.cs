using System;
using MenuNavigation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Loading
{
    public class EndGameScreenView : MenuScreen
    {
        [SerializeField] private Button _mainMenu;
        [SerializeField] private Button _restart;

        public Action OnMainMenuAction;
        public Action OnRestartAction;

        protected override void Start()
        {
            base.Start();
            
            _mainMenu.onClick.AddListener(OnStartGame);
            _restart.onClick.AddListener(OnExitGame);
        }

        void OnStartGame()
        {
            OnMainMenuAction?.Invoke();
        }

        void OnExitGame()
        {
            OnRestartAction?.Invoke();
        }
    }
}