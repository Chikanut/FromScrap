using GameLogic.States.States;
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
            Container.BindState<LoadMenuSceneState>();
            Container.BindState<MainMenuState>();
            Container.BindState<LoadGameSceneState>();
            Container.BindState<LoadGameResourcesState>();
            Container.BindState<InitGameState>();
            Container.BindState<GameplayState>();
            Container.BindState<PauseGameState>();
            Container.BindState<EndGameState>();
            
            Container.BindInterfacesTo<StateMachineController>().AsSingle();
        }
    }
}