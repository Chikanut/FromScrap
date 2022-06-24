namespace ShootCommon.GlobalStateMachine
{
    public enum StateMachineTriggers 
    {
        //InitialStates
        LoadInfo,
        
        //MenusStates
        LoadMenuScene,
        MainMenu,
        
        //GameStates
        LoadGameScene,
        LoadGameResources,
        InitGame,
        StartGame,
        Game,
        Pause,
        EndGame,
        Results
    }
}