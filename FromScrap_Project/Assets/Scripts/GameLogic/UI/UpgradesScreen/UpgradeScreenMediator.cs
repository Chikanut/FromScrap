using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using Zenject;

namespace UI.Upgrades
{
	public class UpgradeScreenMediator : Mediator<UpgradeScreenView>
	{
		[Inject]
		public void Init(ISignalService signalService)
		{

		}

		protected override void OnMediatorInitialize()
		{
			base.OnMediatorInitialize();
		}
	}
}