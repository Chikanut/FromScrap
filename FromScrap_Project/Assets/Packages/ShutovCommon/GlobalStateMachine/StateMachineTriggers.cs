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
        SpawnGameResources,
        InitGame,
        StartGame,
        Game,
        Pause,
        EndGame,
        Results
    }
}