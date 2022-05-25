using UnityEngine;
using System;
using ShootCommon.GlobalStateMachine;

namespace Visartech.Progress
{
    [Serializable]
    public class Progress : ProgressBase<Progress>
    {

        [Serializable]
        public class PlayerData : Observable
        {
            public PlayerData()
            {
                Car = 0;
                Level = 0;
            }

            public int Car;
            public int Level;
            public int Experience;
            public int Scrap;
        }

        [Serializable]
        public class GameData
        {
            public bool SoundEnabled = true;
            public bool MusicEnabled = true;
        }

        [Serializable]
        public class DevelopmentData
        {
            public bool isTesting;
            public string testSceneName;
            public StateMachineTriggers testState;
        }

        [Serializable]
        public class StatisticsData
        {
            public bool isFirstPlay = false;
            public int KillsRecord;
            public int DamageRecord;
            public int LevelRecord;
            public int TimeRecord;
        }

        private PlayerData _playerData;
        private GameData _gameData = new GameData();
        private DevelopmentData _developmentData = new DevelopmentData();
        private StatisticsData _statisticsData = new StatisticsData();

        public static PlayerData Player
        {
            get => instance._playerData;
            set => instance._playerData = value;
        }

        public static GameData Game {
            get => instance._gameData;
            set => instance._gameData = value;
        }
        
        public static DevelopmentData Development {
            get => instance._developmentData;
            set => instance._developmentData = value;
        }
        
        public static StatisticsData Statistics {
            get => instance._statisticsData;
            set => instance._statisticsData = value;
        }

        public void Reset()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            ClearAllFields();
        }
    }
}