using Packages.Common.StateMachineGlobal;
using Packages.Common.Storage;
using Packages.Utils.SoundManager;
using ShootCommon.Signals;
using Zenject;

namespace Common
{
    public class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            MessageBrokerInstaller.Install(Container);
            SignalBusInstaller.Install(Container);
            SignalsInstaller.Install(Container);
            
            SoundInstaller.Install(Container);
            GlobalStateMachineInstaller.Install(Container);
            StorageInstaller.Install(Container);
            GamePlayInstaller.Install(Container);
        }
    }
}