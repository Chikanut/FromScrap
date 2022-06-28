using Configs.GameResourcesManagerConfig.Controllers;
using GameLogic.GameResourcesLogic.Controllers;
using GameLogic.GameResourcesLogic.GameResourcesSceneController;
using ShootCommon.Views.Mediation;
using Zenject;

namespace GameLogic.GameResourcesLogic
{
    public class GameResourcesManagerInstaller : Installer<GameResourcesManagerInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameResourcesManagerConfigController>().AsSingle();
            Container.BindInterfacesTo<GameResourcesLoaderController>().AsSingle();
            Container.BindViewToMediator<GameResourcesSceneView, GameResourcesSceneMediator>();
        }
    }
}
