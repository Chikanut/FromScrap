using Zenject;

namespace Packages.Utils.SoundManager
{
    public class SoundInstaller : Installer<SoundInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SoundController>().AsSingle();
        }
    }
}