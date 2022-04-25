using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using Signals;
using UnityEngine;
using Zenject;

namespace UI.MainMenu
{
    public class MainManuScreenMadiator : Mediator<MainMenuScreenView>
    {
        private ISignalService _signalService;
        
        [Inject]
        public void Init(ISignalService signalService)
        {
            _signalService = signalService;
        }
    
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

           View.OnStartGameAction = OnStartGame;
           View.OnExitAction = OnGameExit;
        }
        
        private void OnStartGame()
        {
            _signalService.Publish(new StartGameSignal());
        }
        
        private void OnGameExit()
        {
            Application.Quit();
        }
    }
}