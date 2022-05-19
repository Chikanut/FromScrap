using ShootCommon.GlobalStateMachine;
using ShootCommon.GlobalStateMachine.States;
using ShootCommon.Signals;
using UniRx;
using Unity.Entities;
using Zenject;

public partial class GameManagerSystem : SystemBase
{
    private IGameDataController _gameDataController;
    private readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();

    private bool _updateGameData;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        ProjectContext.Instance.Container.Inject(this);
    }
    
    [Inject]
    public void Init(IGameDataController gameDataController, ISignalService signalService)
    {
        _gameDataController = gameDataController;
        
        signalService.Receive<ChangeStateSignal>().Subscribe((stateSignal) =>
        {
            switch (stateSignal.SelectedState)
            {
                case StateMachineTriggers.InitGame:
                    PrepareGame();
                    break;
                case StateMachineTriggers.Game:
                    StartGame();
                    break;
                case StateMachineTriggers.EndGame:
                    EndGame();
                    break;
            }
        }).AddTo(_disposeOnDestroy);
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _disposeOnDestroy.Dispose();
    }

    public void PrepareGame()
    {
        _gameDataController.Initialize();
    }

    public void StartGame()
    {
        _updateGameData = true;
    }

    public void EndGame()
    {
        _updateGameData = false;
    }

    protected override void OnUpdate()
    {
        if (_updateGameData)
            _gameDataController.Update(Time.DeltaTime);
    }
}
