using GameLogic.States.States;
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
            
            Container.BindState<LoadInfoState>();
            Container.BindState<MainMenuState>();
            Container.BindState<LoadGameSceneState>();
            Container.BindState<InitGameState>();
            
            Container.BindInterfacesTo<StateMachineController>().AsSingle();
        }
    }
}