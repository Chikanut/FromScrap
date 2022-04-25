using System;
using MenuNavigation;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.MainMenu
{
    public class MainMenuScreenView : MenuScreen
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitButton;

        public Action OnStartGameAction;
        public Action OnExitAction;

        protected override void Start()
        {
            base.Start();
            
            _startGameButton.onClick.AddListener(OnStartGame);
            _exitButton.onClick.AddListener(OnExitGame);
        }

        void OnStartGame()
        {
            OnStartGameAction?.Invoke();
        }

        void OnExitGame()
        {
            OnExitAction?.Invoke();
        }
    }
}