using ShootCommon.Views.Mediation;
using UI.Upgrades;
using Zenject;

namespace MenuNavigation
{
    public class MenuNavigationInstaller : Installer<MenuNavigationInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuNavigationController>().AsSingle();

            Container.BindViewToMediator<GamePlayScreenView, GamePlayScreenMediator>();
            Container.BindViewToMediator<UpgradeScreenView, UpgradeScreenMediator>();
        }
    }
}
