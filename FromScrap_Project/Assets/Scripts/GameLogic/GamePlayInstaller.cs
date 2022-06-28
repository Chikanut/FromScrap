using Configs.Gameplay.Controllers;
using GameLogic.GameResourcesLogic.Controllers;
using Zenject;

namespace Packages.Common.StateMachineGlobal
{
    public class GamePlayInstaller : Installer<GamePlayInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<EnemiesSpawnerSystem>().AsSingle();
            Container.BindInterfacesTo<GameDataController>().AsSingle();
            Container.BindInterfacesTo<GameManagerSystem>().AsSingle();
            Container.BindInterfacesTo<GameplayLevelsConfigController>().AsSingle();
            Container.BindInterfacesTo<GameResourcesSpawnerController>().AsSingle();
        }
    }
}