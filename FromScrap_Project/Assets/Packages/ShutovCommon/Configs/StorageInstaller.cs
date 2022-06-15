using Packages.Common.Storage.Config;
using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.EnemySpawner;
using Packages.Common.Storage.Config.Upgrades;
using Zenject;

namespace Packages.Common.Storage
{
    public class StorageInstaller: Installer<StorageInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<EnemySpawnerConfigController>().AsSingle();
            Container.BindInterfacesTo<CarsConfigController>().AsSingle();
            Container.BindInterfacesTo<SoundConfigController>().AsSingle();
            Container.BindInterfacesTo<PlayerProgressionConfigController>().AsSingle();
            Container.BindInterfacesTo<UpgradesConfigController>().AsSingle();
        }
    }
}