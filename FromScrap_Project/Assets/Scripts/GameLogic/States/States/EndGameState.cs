using MenuNavigation;
using Packages.Common.Storage.Config;
using ShootCommon.GlobalStateMachine;
using Signals;
using Stateless;
using UI.Screens.Loading;
using UnityEngine;
using Visartech.Progress;
using Zenject;

namespace GameLogic.States.States
{
    public class EndGameState : GlobalState
    {
        protected override void Configure()
        {
            Permit<LoadMenuSceneState>(StateMachineTriggers.LoadMenuScene);
            Permit<LoadGameSceneState>(StateMachineTriggers.LoadGameScene);
        }
        
        protected override void OnEntry(StateMachine<IState, StateMachineTriggers>.Transition transition = null)
        {
            _menuNavigationController.HideAllMenuScreens();
            _menuNavigationController.ShowMenuScreen<EndGameScreenView>(()=>
            {
                SubscribeToSignals();
                UpdateResults();
                Time.timeScale = 0;
            }, "EndGameScreen");
        }
        
        private void SubscribeToSignals()
        {
            SubscribeToSignal<GoToMainMenuSignal>(GoToMainMenu);
            SubscribeToSignal<RestartGameSignal>(RestartGame);
        }

        void UpdateResults()
        {
            UpdateStats();
            UpdateLevel();
            UpdateScrap();
        }

        void UpdateStats()
        {
            if (Progress.Statistics.KillsRecord < _gameDataController.Data.Stats.Kills)
            {
                Progress.Statistics.KillsRecord = _gameDataController.Data.Stats.Kills;
            }
            
            if (Progress.Statistics.DamageRecord < _gameDataController.Data.Stats.Damage)
            {
                Progress.Statistics.DamageRecord = _gameDataController.Data.Stats.Damage;
            }
            
            if (Progress.Statistics.TimeRecord < _gameDataController.Data.Stats.Time)
            {
                Progress.Statistics.TimeRecord = (int)_gameDataController.Data.Stats.Time;
            }
            
            if (Progress.Statistics.LevelRecord < _gameDataController.Data.Stats.Level)
            {
                Progress.Statistics.LevelRecord = _gameDataController.Data.Stats.Level;
            }
        }

        void UpdateLevel()
        {
            var currentExperience = Progress.Player.Experience + _gameDataController.Data.Stats.ExperienceGained;
            var level = Progress.Player.Level;

            while (currentExperience > 0)
            {
                if (_playerProgressionConfigController.GetPlayerProgressionData.LevelsExperience[level] <=
                    currentExperience)
                {
                    currentExperience -=
                        _playerProgressionConfigController.GetPlayerProgressionData.LevelsExperience[level];
                    level++;
                }
                else
                    break;
            }

            Progress.Player.Experience = currentExperience;
            Progress.Player.Level = level;
        }

        void UpdateScrap()
        {
            Progress.Player.Scrap += _gameDataController.Data.Stats.CollectedScrap;
        }

        private IMenuNavigationController _menuNavigationController;
        private IGameDataController _gameDataController;
        private IPlayerProgressionConfigController _playerProgressionConfigController;
        
        [Inject]
        public void Init(IMenuNavigationController menuNavigationController, IGameDataController gameDataController, IPlayerProgressionConfigController playerProgressionConfigController)
        {
            _menuNavigationController = menuNavigationController;
            _gameDataController = gameDataController;
            _playerProgressionConfigController = playerProgressionConfigController;
        }
        
        void GoToMainMenu(GoToMainMenuSignal signal)
        {
            _menuNavigationController.HideAllMenuScreens();

            ECS_Logic_Extentions.ClearScene(() =>
            {
                Time.timeScale = 1;
                Fire(StateMachineTriggers.LoadMenuScene);
            });
        }

        void RestartGame(RestartGameSignal signal)
        {
            _menuNavigationController.HideAllMenuScreens();
            
            ECS_Logic_Extentions.ClearScene(() =>
            {
                Time.timeScale = 1;
                Fire(StateMachineTriggers.LoadGameScene);
            });
        }
    }
}