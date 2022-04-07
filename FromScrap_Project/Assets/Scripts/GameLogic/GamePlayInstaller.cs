using Packages.Common.StateMachineGlobal.States;
using ShootCommon.GlobalStateMachine;
using Zenject;

namespace Packages.Common.StateMachineGlobal
{
    public class GamePlayInstaller : Installer<GamePlayInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<EnemiesSpawner>().AsSingle();
        }
    }
}