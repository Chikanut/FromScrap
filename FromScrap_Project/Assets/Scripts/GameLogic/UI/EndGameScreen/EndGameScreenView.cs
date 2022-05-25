using System;
using DG.Tweening;
using MenuNavigation;
using Packages.Common.Storage.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visartech.Progress;

namespace UI.Screens.Loading
{
    public class EndGameScreenView : MenuScreen
    {
        [Header("XPerience")]
        [SerializeField] private Slider _levelProgressBar;
        [SerializeField] private TextMeshProUGUI _newXP;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _xpProgress;
        [Header("Scrap")]
        [SerializeField] private TextMeshProUGUI _newScrap;
        [SerializeField] private TextMeshProUGUI _scrap;
        [Header("Upgrades")]
        [SerializeField] private UpgradesInfoPanelSeparated _upgradesInfo;
        [Header("Stats")]
        [SerializeField] private StatisticPanelView _killsStats;
        [SerializeField] StatisticPanelView _damageStats;
        [SerializeField] StatisticPanelView _levelStats;
        [SerializeField] StatisticPanelView _timeStats;
        [Header("Buttons")]
        [SerializeField] private Button _mainMenu;
        [Header("Settings")]
        [SerializeField] private float _initTime = 0.8f;
        
        public Action OnMainMenuAction;

        private Vector2 _startScrapPosition;
        Vector2 _startExperiencePosition;

        protected override void Start()
        {
            base.Start();

            _startScrapPosition = _newScrap.rectTransform.anchoredPosition;
            _startExperiencePosition = _xpProgress.rectTransform.anchoredPosition;

            _mainMenu.onClick.AddListener(OnStartGame);
        }

        void OnStartGame()
        {
            OnMainMenuAction?.Invoke();
        }

        public void UpdateInfo(CurrentCarInfoData carInfo)
        {
            _upgradesInfo.UpdateInfo(carInfo);
        }

        public void UpdateStats(CurrentGameStats stats)
        {
            _killsStats.SetInfo(StatisticPanelView.StatisticType.number, stats.Kills, Progress.Statistics.KillsRecord);
            _damageStats.SetInfo(StatisticPanelView.StatisticType.number, stats.Damage,
                Progress.Statistics.DamageRecord);
            _levelStats.SetInfo(StatisticPanelView.StatisticType.number, stats.Level+1, Progress.Statistics.LevelRecord+1);
            _timeStats.SetInfo(StatisticPanelView.StatisticType.time, (int) stats.Time, Progress.Statistics.TimeRecord);
        }

        private Sequence _scrapView;

        public void UpdateScrap(int scrapGathered)
        {
            _scrapView?.Kill();
       
            _newScrap.rectTransform.anchoredPosition = _startScrapPosition;
            _newScrap.color = Color.white;

            _scrap.text = Progress.Player.Scrap.ToString();
            _newScrap.text = "+" + scrapGathered;

            if (scrapGathered <= 0)
            {
                _newScrap.color = Color.clear;
                return;
            }

            _scrapView = DOTween.Sequence();

            _scrapView.Insert(_initTime, _newScrap.rectTransform.DOAnchorPosY(_startScrapPosition.y - 15, 2f));
            _scrapView.Insert(_initTime, _newScrap.DOFade(0, 2f));
            _scrapView.Insert(_initTime+0.3f,
                _scrap.DOCounter(Progress.Player.Scrap, Progress.Player.Scrap + scrapGathered, 2f));
            
            _scrapView.SetUpdate(true);
        }


        public void UpdateXP(int xpGained, PlayerProgressionConfigData config)
        {
            _levelSequence?.Kill();
            _levelSequence = DOTween.Sequence();
            
            _newXP.text = "+" + xpGained + " <sup>XP</sup>";
            _levelProgressBar.value =
                (float) Progress.Player.Experience / config.LevelsExperience[Progress.Player.Level];
            _level.text = (Progress.Player.Level + 1).ToString();

            VisualiseExperienceBar(xpGained, config, 2f);
        }

        Sequence _levelSequence;

        void VisualiseExperienceBar(int xpGained, PlayerProgressionConfigData config, float time)
        {
            _xpProgress.color = Color.clear;
            _xpProgress.rectTransform.anchoredPosition += Vector2.down * 25;

            var maxExperience = Progress.Player.Experience + xpGained;
            var maxLevel = Progress.Player.Level;

            while (maxExperience > 0)
            {
                if (config.LevelsExperience[maxLevel] <= maxExperience)
                {
                    maxExperience -= config.LevelsExperience[maxLevel];
                    maxLevel++;
                }
                else
                {
                    break;
                }
            }

            var startTime = maxLevel - Progress.Player.Level > 0 ? time * 0.75f : 0;
            var animTime = maxLevel - Progress.Player.Level > 0 ? time / (maxLevel - Progress.Player.Level): time;
            var animNum = 0;

            for (var i = Progress.Player.Level; i < maxLevel; i++)
            {
                var tween = _levelProgressBar.DOValue(1, animTime);
                var i1 = i;
                tween.onComplete = () =>
                {
                    _levelProgressBar.value = 0;
                    _level.text = (i1 + 1).ToString();
                };
                tween.SetEase(Ease.InCubic);
                _levelSequence.Insert(_initTime + animNum * animTime, tween);
                animNum++;
            }

            _xpProgress.text = $"{maxExperience}|{config.LevelsExperience[maxLevel]}";
            
            var color =  Color.white;
            color.a = 0;
            _xpProgress.color = color;

            _levelSequence.Insert(_initTime + startTime,
                _levelProgressBar.DOValue((float) maxExperience / config.LevelsExperience[maxLevel], animTime * 0.25f)
                    .SetEase(Ease.OutCubic));
            _levelSequence.Insert(_initTime + startTime, _xpProgress.rectTransform.DOAnchorPosY(_startExperiencePosition.y,
                animTime * 0.25f));
            _levelSequence.Insert(_initTime + startTime, _xpProgress.DOFade(1, animTime * 0.25f));
            
            _levelSequence.SetUpdate(true);
        }
    }
}