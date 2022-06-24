using Configs.GameResourcesManagerConfig.Controllers;
using Zenject;

namespace GameLogic.GameResourcesLogic
{
    public class GameResourcesManagerInstaller : Installer<GameResourcesManagerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameResourcesManagerConfigController>().AsSingle();
            Container.BindInterfacesTo<GameResourcesLoaderController>().AsSingle();
        }
    }
}
