using ShootCommon.Views.Mediation;
using UI.Loading;
using UI.MainMenu;
using UI.Preloader;
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
            Container.BindViewToMediator<PreloaderScreenView, PreloaderScreenMediator>();
            Container.BindViewToMediator<LoadingScreenView, LoadingScreenMediator>();
            Container.BindViewToMediator<MainMenuScreenView, MainManuScreenMadiator>();
        }
    }
}
