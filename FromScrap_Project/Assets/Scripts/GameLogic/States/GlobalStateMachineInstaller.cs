using Packages.Common.StateMachineGlobal.States;
using ShootCommon.GlobalStateMachine;
using Zenject;

namespace Packages.Common.StateMachineGlobal
{
    public class GlobalStateMachineInstaller : Installer<GlobalStateMachineInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindState<InitState>();
            
            Container.BindState<StartState>();
            Container.BindState<InitGameState>();
            
            Container.BindInterfacesTo<StateMachineController>().AsSingle();
        }
    }
}