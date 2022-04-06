using Packages.Common.Storage.Config;
using Zenject;

namespace Packages.Common.Storage
{
    public class StorageInstaller: Installer<StorageInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BaseConfigController>().AsSingle();
        }
    }
}