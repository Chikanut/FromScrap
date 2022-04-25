using ShootCommon.Views.Mediation;
using Signals;

namespace UI.PopUps.Pause
{
    public class PausePopUpMediator : Mediator<PausePopUpView>
    {
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

            View.OnContinueAction = OnContinueAction;
            View.OnMainMenuAction = OnMainMenuAction;
            View.OnRestartAction = OnRestartAction;
        }

        private void OnContinueAction()
        {
            SignalService.Publish(new ContinueGameSignal());
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