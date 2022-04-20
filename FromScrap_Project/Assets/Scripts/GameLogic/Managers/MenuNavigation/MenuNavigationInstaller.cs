using ShootCommon.Views.Mediation;
using Zenject;

namespace MenuNavigation
{
    public class MenuNavigationInstaller : Installer<MenuNavigationInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<MenuNavigationController>().AsSingle();

            Container.BindViewToMediator<GamePlayScreenView, GamePlayScreenMediator>();
        }
    }
}
