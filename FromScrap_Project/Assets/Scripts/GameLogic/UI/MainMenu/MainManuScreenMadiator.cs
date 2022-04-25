using ShootCommon.Views.Mediation;
using Signals;
using UnityEngine;

namespace UI.Screens.MainMenu
{
    public class MainManuScreenMadiator : Mediator<MainMenuScreenView>
    {
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

           View.OnStartGameAction = OnStartGame;
           View.OnExitAction = OnGameExit;
        }
        
        private void OnStartGame()
        {
            SignalService.Publish(new StartGameSignal());
        }
        
        private void OnGameExit()
        {
            Application.Quit();
        }
    }
}