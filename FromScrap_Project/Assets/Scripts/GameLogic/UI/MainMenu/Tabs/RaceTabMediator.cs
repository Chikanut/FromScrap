using ShootCommon.Views.Mediation;
using Signals;

namespace UI.Screens.MainMenu.Tabs
{
    public class RaceTabMediator : Mediator<RaceTab>
    {
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();
            View.OnStartGameAction += OnStartGame;
        }

        protected override void OnMediatorEnable()
        {
            base.OnMediatorEnable();
            
            SignalService.Publish(new OnMainMenuChangeViewAction()
            {
                ViewName = View.TabName
            });
        }

        private void OnStartGame()
        {
            SignalService.Publish(new StartGameSignal());
        }
    }
}