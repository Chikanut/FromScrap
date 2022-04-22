using Cars.View.Components;
using MenuNavigation;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.GlobalStateMachine;
using ShootCommon.GlobalStateMachine.States;
using ShootCommon.Signals;
using UniRx;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Visartech.Progress;
using Zenject;

public partial class GameManagerSystem : SystemBase
{
    private readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();
    
    private ICarsConfigController _carsConfigController;
    private IMenuNavigationController _menuNavigationController;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        ProjectContext.Instance.Container.Inject(this);
    }
    
    protected override void OnDestroy()
    {
        _disposeOnDestroy.Dispose();
        
        base.OnDestroy();
    }

    private ISignalService _signalService;
    [Inject]
    public void Init(ISignalService signalService, ICarsConfigController carsConfigController, IMenuNavigationController menuNavigationController)
    {
        _carsConfigController = carsConfigController;
        _signalService = signalService;
        _menuNavigationController = menuNavigationController;
        
        signalService.Receive<ChangeStateSignal>().Subscribe((stateSignal) =>
        {
            if(stateSignal.SelectedState == StateMachineTriggers.InitGame)
                InitGame();
        }).AddTo(_disposeOnDestroy);
        
       
    }

    void InitGame()
    {
        SpawnPlayer();
        _menuNavigationController.ShowMenuScreen<GamePlayScreenView>(null, "GamePlayScreen");
    }

    void SpawnPlayer()
    {
        var carData = _carsConfigController.GetCarData(Progress.Player.CurrentCar);
        
        EntityPoolManager.Instance.GetObject(carData.Prefab, (entity, manager) =>
        {
            manager.AddComponentData(entity, new CarIDComponent() {ID = Progress.Player.CurrentCar});
            manager.SetComponentData(entity, new Translation()
            {
                Value = new float3(0,3,0)
            });
        });
    }

    protected override void OnUpdate()
    {
        
    }
}
