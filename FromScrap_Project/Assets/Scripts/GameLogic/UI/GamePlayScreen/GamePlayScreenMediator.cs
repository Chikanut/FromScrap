using System.Collections.Generic;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.Signals;
using ShootCommon.Views.Mediation;
using UniRx;
using UnityEngine;
using Visartech.Progress;
using Zenject;

public class GamePlayScreenMediator : Mediator<GamePlayScreenView>
{
    int _currentLevel;
    private List<int> _levelsInfo = new List<int>();

    private ICarsConfigController _carsConfigController;
    
    [Inject]
    public void Init(ISignalService signalService, ICarsConfigController carsConfigController)
    {
        _carsConfigController = carsConfigController;
        
        signalService.Receive<OnExperienceChangeSignal>().Subscribe(OnExperienceGathered).AddTo(DisposeOnDestroy);
        signalService.Receive<OnLevelUpSignal>().Subscribe(OnLevelUp).AddTo(DisposeOnDestroy);
    }
    
    protected override void OnMediatorInitialize()
    {
        base.OnMediatorInitialize();

        _currentLevel = 0;
        View.SetCurrentLevel(_currentLevel);
        _levelsInfo = _carsConfigController.GetCarData(Progress.Player.CurrentCar).LevelsExperience;
    }

    private void OnLevelUp(OnLevelUpSignal signal)
    {
        _currentLevel = signal.Level;
        View.SetCurrentLevel(_currentLevel);
    }

    private void OnExperienceGathered(OnExperienceChangeSignal signal)
    {
        View.SetExperience(signal.Experience / (float)_levelsInfo[_currentLevel]);

     //   Debug.Log("Current level :" + _currentLevel + " Current experience : " + signal.Experience +
                 // " Target experience : " + _levelsInfo[_currentLevel]);
    }



}
