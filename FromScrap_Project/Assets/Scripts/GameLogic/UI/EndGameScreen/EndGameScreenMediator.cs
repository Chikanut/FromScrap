﻿using Packages.Common.Storage.Config;
using Packages.Common.Storage.Config.Upgrades;
using ShootCommon.Views.Mediation;
using Signals;
using Visartech.Progress;
using Zenject;

namespace UI.Screens.Loading
{
    public class EndGameScreenMediator : Mediator<EndGameScreenView>
    {
        private IGameDataController _gameDataController;
        private IPlayerProgressionConfigController _playerProgressionConfigController;
        IUpgradesConfigController _upgradesConfigController;
        
        [Inject]
        public void Init(IGameDataController gameDataController, IPlayerProgressionConfigController playerProgressionConfigController, IUpgradesConfigController upgradesConfigController)
        {
            _gameDataController = gameDataController;
            _playerProgressionConfigController = playerProgressionConfigController;
            _upgradesConfigController = upgradesConfigController;
        }
        
        protected override void OnMediatorInitialize()
        {
            base.OnMediatorInitialize();

           View.OnMainMenuAction = OnMainMenuAction;
        }
        
        protected override void OnMediatorEnable()
        {
            base.OnMediatorEnable();
            
            View.UpdateInfo(_gameDataController.Data.CarData, _upgradesConfigController.GetUpgradesData);
            var stats = _gameDataController.Data.Stats;
            View.UpdateStats(stats.Kills, Progress.Statistics.KillsRecord,
                stats.Damage, Progress.Statistics.DamageRecord,
                stats.Level, Progress.Statistics.LevelRecord,
                stats.Time, Progress.Statistics.TimeRecord);
            View.UpdateScrap(_gameDataController.Data.Stats.CollectedScrap, Progress.Player.Scrap);
            View.UpdateXP(Progress.Player.Level, Progress.Player.Experience ,_gameDataController.Data.Stats.ExperienceGained,
                _playerProgressionConfigController.GetPlayerProgressionData.LevelsExperience);
        }
        
        private void OnMainMenuAction()
        {
            SignalService.Publish(new GoToMainMenuSignal());
        }
    }
}