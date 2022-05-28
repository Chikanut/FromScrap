using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.MainMenu.Tabs
{
    public class RaceTab : MainMenuTab
    {
        [Header("Components")]
        [SerializeField] private Button _startGameButton;
        public Action OnStartGameAction;
        
        protected override void Start()
        {
            base.Start();
            
            _startGameButton.onClick.AddListener(OnStartGame);
        }

        void OnStartGame()
        {
            OnStartGameAction?.Invoke();
        }
    }
}