using Cars.View.Components;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.Views.Mediation;
using UniRx;
using Unity.Transforms;
using UnityEngine;
using Visartech.Progress;
using Zenject;

public class MainMenuBackTransitionsMediator : Mediator<MainMenuBackTransitionsView>
{
    private ICarsConfigController _carsConfigController;
    
    [Inject]
    public void Init(ICarsConfigController carsConfigController)
    {
        _carsConfigController = carsConfigController;
    }
    
    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();

        SignalService.Receive<OnMainMenuChangeViewAction>().Subscribe(OnMainMenuStateChanged).AddTo(DisposeOnDestroy);
        SpawnCar();
    }

    protected override void OnMediatorEnable()
    {
        base.OnMediatorEnable();
        
        Debug.LogError("Enable");
    }

    void SpawnCar()
    {
        if(View.CarPosition == null)
            return;
        
        Debug.LogError("Spawn car");
        var carData = _carsConfigController.GetCarData(Progress.Player.Car);
        
        EntityPoolManager.Instance.GetObject(carData.PresentationPrefab, (entity, manager) =>
        {
            manager.AddComponentData(entity, new CarIDComponent() {ID = Progress.Player.Car});
            manager.SetComponentData(entity, new Translation()
            {
                Value = View.CarPosition.position
            });
        });
    }

    private void OnMainMenuStateChanged(OnMainMenuChangeViewAction obj)
    {
        View.OnStateChanged(obj.ViewName);
    }
}
