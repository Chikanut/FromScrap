﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using FromScrap.Tools;
using MenuNavigation;
using Packages.Common.Storage.Config.Upgrades;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Loading
{
    public class EndGameScreenView : MenuScreen
    {
        [Header("XPerience")] [SerializeField] private Slider _levelProgressBar;
        [SerializeField] private TextMeshProUGUI _newXP;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _xpProgress;
        [SerializeField] private float _experienceAnimationTime = 2f;
        [Header("Scrap")] [SerializeField] private TextMeshProUGUI _newScrap;
        [SerializeField] private TextMeshProUGUI _scrap;
        [Header("Upgrades")] [SerializeField] private UpgradesInfoPanelSeparated _upgradesInfo;
        [Header("Stats")] [SerializeField] private StatisticPanelView _killsStats;
        [SerializeField] StatisticPanelView _damageStats;
        [SerializeField] StatisticPanelView _levelStats;
        [SerializeField] StatisticPanelView _timeStats;
        [Header("Buttons")] [SerializeField] private Button _mainMenu;
        [Header("Settings")] [SerializeField] private float _initTime = 0.8f;

        public Action OnMainMenuAction;

        [SerializeField] Vector2 _startScrapPosition;
        [SerializeField] Vector2 _startExperiencePosition;

        protected override void Start()
        {
            base.Start();

            _mainMenu.onClick.AddListener(OnStartGame);
        }

        
        protected override void OnEnable()
        {
            _newXP.text = "0";
            _level.text = "0";
            _xpProgress.text = "0";
            _newScrap.text = "0";
            _scrap.text = "0";
            _levelProgressBar.value = 0;
            _upgradesInfo.ClearAll();
            
            base.OnEnable();
            
            _mainMenu.interactable = false;

            DOVirtual.DelayedCall(_initTime, () =>
            {
                _mainMenu.interactable = true;
            });
        }

        void OnStartGame()
        {
            OnMainMenuAction?.Invoke();
        }

        public void UpdateInfo(CurrentCarInfoData carInfo, UpgradesConfigData upgradesConfigData)
        {
            _upgradesInfo.UpdateInfo(carInfo, upgradesConfigData);
        }

        public void UpdateStats(int currentKills, int recordKills,
            int currentDamage, int recordDamage,
            int currentLevel, int recordLevel,
            float currentTime, int recordTime)
        {
            _killsStats.SetInfo(StatisticType.count, currentKills, recordKills);
            _damageStats.SetInfo(StatisticType.count, currentDamage,
                recordDamage);
            _levelStats.SetInfo(StatisticType.count, currentLevel+1,
                recordLevel+1);
            _timeStats.SetInfo(StatisticType.time, (int) currentTime, recordTime);
        }

        private Sequence _scrapView;

        public void UpdateScrap(int scrapGathered, int currentScrap)
        {
            _scrapView?.Kill();

            _newScrap.rectTransform.anchoredPosition = _startScrapPosition;
            _newScrap.color = Color.white;

            _scrap.text = UI_Extentions.GetValue(currentScrap, StatisticType.count);
            _newScrap.text = "+" + UI_Extentions.GetValue(scrapGathered, StatisticType.count);

            if (scrapGathered <= 0)
            {
                _newScrap.color = Color.clear;
                return;
            }

            _scrapView = DOTween.Sequence();
            _scrapView.SetUpdate(true);

            _scrapView.Insert(_initTime, _newScrap.rectTransform.DOAnchorPosY(_startScrapPosition.y - 15, 2f));
            _scrapView.Insert(_initTime, _newScrap.DOFade(0, 2f));
            _scrapView.Insert(_initTime + 0.3f,
                _scrap.DOCounter(currentScrap, currentScrap + scrapGathered, 2f, true));


        }

        Sequence _levelSequence;

        public void UpdateXP(int currentLevel, int currentExperience, int gatheredExperience,
            List<int> levelsExperience)
        {
            _levelSequence?.Kill();
            _levelSequence = DOTween.Sequence();
            _levelSequence.SetUpdate(true);
            
            _newXP.text = "+" + gatheredExperience + " <sup>XP</sup>";
            _levelProgressBar.value =
                (float) currentExperience / levelsExperience[currentLevel];
            _level.text = (currentLevel).ToString();

            VisualiseExperienceBar(currentLevel, currentExperience, gatheredExperience, levelsExperience,
                _experienceAnimationTime);
        }

        void VisualiseExperienceBar(int currentLevel, int currentExperience, int gatheredExperience,
            List<int> levelsExperience, float time)
        {
            _xpProgress.color = Color.clear;
            _xpProgress.rectTransform.anchoredPosition = _startExperiencePosition + Vector2.down * 25;

            var maxExperience = currentExperience + gatheredExperience;
            var maxLevel = currentLevel;

            while (maxExperience > 0)
            {
                if (maxLevel >= levelsExperience.Count - 1)
                {
                    maxExperience = math.clamp(maxExperience, 0,
                        levelsExperience[levelsExperience.Count - 1]);
                    break;
                }

                if (levelsExperience[maxLevel] <= maxExperience)
                {
                    maxExperience -= levelsExperience[maxLevel];
                    maxLevel++;
                    maxLevel = math.min(maxLevel, levelsExperience.Count - 1);
                }
                else
                {
                    break;
                }
            }

            var startTime = 0f;
            var animTime = maxLevel - currentLevel > 0 ? time / (maxLevel - currentLevel) : time;
            var animNum = 0;

            for (var i = currentLevel; i < maxLevel; i++)
            {
                var tween = _levelProgressBar.DOValue(1, animTime * 0.9f);
                var i1 = i;
                tween.onComplete = () =>
                {
                    _levelProgressBar.value = 0;
                    _level.text = (i1 + 1).ToString();
                };
                _levelSequence.Insert(_initTime + animNum * animTime, tween);
                animNum++;
                startTime += animTime;
            }

            _xpProgress.text = $"{maxExperience}|{levelsExperience[maxLevel]}";

            var color = Color.white;
            color.a = 0;
            _xpProgress.color = color;

            _levelSequence.Insert(_initTime + startTime,
                _levelProgressBar.DOValue((float) maxExperience / levelsExperience[maxLevel], animTime * 0.25f));
            _levelSequence.Insert(_initTime + startTime, _xpProgress.rectTransform.DOAnchorPos(
                _startExperiencePosition,
                animTime * 0.25f));
            _levelSequence.Insert(_initTime + startTime, _xpProgress.DOFade(1, animTime * 0.25f));
        }
    }
}