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
        InitGame,
        StartGame,
        Game,
        Pause,
        EndGame,
        Results
    }
}