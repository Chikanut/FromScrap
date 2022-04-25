using UnityEngine;
using System;
using System.Linq;
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
                CurrentCar = 0;
            }

            public int CurrentCar;

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

            const string DateFormat = "yyyy.MM.dd";
            public string[] DaysInGame = new string[0];

            public int GetTotalDaysInGame
            {
                get
                {
                    if (DaysInGame.Any(t => t.Equals(DateTime.Now.ToString(DateFormat))))
                        return DaysInGame.Length;

                    AddDay();
                    
                    return DaysInGame.Length;
                }
            }

            public void AddDay()
            {
                var newArr = new string[DaysInGame.Length + 1];

                DaysInGame.CopyTo(newArr, 0);
                newArr[newArr.Length - 1] = DateTime.Now.ToString(DateFormat);
                DaysInGame = newArr;
            }
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