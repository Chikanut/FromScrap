using System.Collections.Generic;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.Views.Mediation;
using Signals;
using UniRx;
using UnityEngine;
using Visartech.Progress;
using Zenject;

namespace UI.Screens.Loading
{
    public class GamePlayScreenMediator : Mediator<GamePlayScreenView>
    {
        int _currentLevel;
        private List<int> _levelsInfo = new List<int>();

        private ICarsConfigController _carsConfigController;

        [Inject]
        public void Init(ICarsConfigController carsConfigController, IGameDataController gameDataController)
        {
            _carsConfigController = carsConfigController;
            
            SignalService.Receive<OnExperienceChangeSignal>().Subscribe(OnExperienceGathered).AddTo(DisposeOnDestroy);
            SignalService.Receive<OnLevelUpSignal>().Subscribe(OnLevelUp).AddTo(DisposeOnDestroy);
            SignalService.Receive<GameTimeChanged>().Subscribe(OnTimeChanged).AddTo(DisposeOnDestroy);
            SignalService.Receive<UpgradesChanged>().Subscribe(OnUpgradesChanged).AddTo(DisposeOnDestroy);
            
            View.UpdateInfo(gameDataController.Data.CarData);
        }

        private void OnTimeChanged(GameTimeChanged obj)
        {
            View.SetTimer(obj.Time);
        }
        
        private void OnUpgradesChanged(UpgradesChanged obj)
        {
            View.UpdateInfo(obj.CarData);
        }
        
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

            _currentLevel = 0;
            View.SetCurrentLevel(_currentLevel);
            _levelsInfo = _carsConfigController.GetCarData(Progress.Player.CurrentCar).LevelsExperience;
            
            View.PauseAction = PauseAction;
        }

        private void PauseAction()
        {
            SignalService.Publish(new OnPauseSignal());
        }

        private void OnLevelUp(OnLevelUpSignal signal)
        {
            _currentLevel = signal.Level;
            View.SetCurrentLevel(_currentLevel);
        }

        private void OnExperienceGathered(OnExperienceChangeSignal signal)
        {
            View.SetExperience(signal.Experience / (float) _levelsInfo[_currentLevel]);
        }
    }
}
