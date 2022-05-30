using ShootCommon.Views.Mediation;
using UI.PopUps.Pause;
using UI.Screens.Loading;
using UI.Screens.MainMenu;
using UI.Screens.MainMenu.Tabs;
using UI.Screens.Preloader;
using UI.Screens.Upgrades;
using Zenject;

namespace MenuNavigation
{
    public class MenuNavigationInstaller : Installer<MenuNavigationInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuNavigationController>().AsSingle();

            //Screens
            Container.BindViewToMediator<GamePlayScreenView, GamePlayScreenMediator>();
            Container.BindViewToMediator<UpgradeScreenView, UpgradeScreenMediator>();
            Container.BindViewToMediator<PreloaderScreenView, PreloaderScreenMediator>();
            Container.BindViewToMediator<LoadingScreenView, LoadingScreenMediator>();
            Container.BindViewToMediator<EndGameScreenView, EndGameScreenMediator>();
            
            //Main menu with tabs
            Container.BindViewToMediator<MainMenuScreenView, MainManuScreenMediator>();
            Container.BindViewToMediator<RaceTab, RaceTabMediator>();
            Container.BindViewToMediator<UpgradesTab, UpgradesTabMediator>();
            Container.BindViewToMediator<SettingsTab, SettingsTabMediator>();
            Container.BindViewToMediator<MainMenuBackTransitionsView, MainMenuBackTransitionsMediator>();
            
            //PopUps
            Container.BindViewToMediator<PausePopUpView, PausePopUpMediator>();
        }
    }
}
