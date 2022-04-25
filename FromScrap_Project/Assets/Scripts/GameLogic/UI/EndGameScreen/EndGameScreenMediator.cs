using ShootCommon.Views.Mediation;
using Signals;

namespace UI.Screens.Loading
{
    public class EndGameScreenMediator : Mediator<EndGameScreenView>
    {
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

           View.OnMainMenuAction = OnMainMenuAction;
           View.OnRestartAction = OnRestartAction;
        }
        
        private void OnMainMenuAction()
        {
            SignalService.Publish(new GoToMainMenuSignal());
        }
        
        private void OnRestartAction()
        {
            SignalService.Publish(new RestartGameSignal());
        }
    }
}