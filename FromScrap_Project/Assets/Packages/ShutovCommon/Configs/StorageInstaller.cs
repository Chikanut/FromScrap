using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.EnemySpawner;
using Zenject;

namespace Packages.Common.Storage
{
    public class StorageInstaller: Installer<StorageInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<EnemySpawnerConfigController>().AsSingle();
            Container.BindInterfacesTo<CarsConfigController>().AsSingle();
        }
    }
}