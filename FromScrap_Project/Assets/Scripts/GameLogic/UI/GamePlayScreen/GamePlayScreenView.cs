using System;
using DG.Tweening;
using MenuNavigation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Loading
{
    public class GamePlayScreenView : MenuScreen
    {
        [Header("Components")]
        [SerializeField] private Button _pause;
        [SerializeField] private Slider _experienceSlider;
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private TextMeshProUGUI _nextLevelLabel;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private UpgradesInfoPanelPiramid _upgradesPanelPyramid;
        
        [SerializeField] CanvasGroup _scrapPanel;
        [SerializeField] TextMeshProUGUI _scrapText;

        [Header("Settings")]
        [SerializeField] private float _experienceFillSpeed;
        [SerializeField] private Ease _experienceFillEasing;
        
        private Sequence _experienceFillSequence;

        public Action PauseAction;

        protected override void Start()
        {
            base.Start();
            
            _pause.onClick.AddListener(OnPause);
        }

        protected override void OnEnable()
        {
            SetCurrentLevel(0);
            SetExperience(0);
            SetTimer(0);
            
            base.OnEnable();
        }

        void OnPause()
        {
            PauseAction?.Invoke();
        }

        private Sequence _scrapPanelSequence;
        
        public void OnScrap(int prevValue, int addedValue)
        {
            _scrapPanelSequence?.Kill();
            _scrapPanelSequence = DOTween.Sequence();
            
            _scrapPanelSequence.Append(_scrapPanel.DOFade(1, 0.5f));
            _scrapPanelSequence.Insert(0, _scrapText.DOCounter(prevValue, addedValue, 0.5f, true));
            _scrapPanelSequence.AppendInterval(2f);
            _scrapPanelSequence.Append(_scrapPanel.DOFade(0, 1));
        }

        public void UpdateInfo(CurrentCarInfoData carInfo)
        {
            _upgradesPanelPyramid.UpdateInfo(carInfo);
        }

        public void SetTimer(float seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            var str = time.ToString(@"mm\:ss");
            
            _time.text = str;
        }

        public void SetExperience(float value)
        {
            value = Mathf.Clamp01(value);

            _experienceFillSequence?.Kill();
            _experienceFillSequence = DOTween.Sequence();

            if (value > _experienceSlider.value)
            {
                var fillTime = (value - _experienceSlider.value) / _experienceFillSpeed;
                _experienceFillSequence.Insert(0,
                    _experienceSlider.DOValue(value, fillTime).SetEase(_experienceFillEasing));
                _experienceFillSequence.Play();
            }
            else
            {
                _experienceSlider.value = value;
            }
        }

        public void SetCurrentLevel(int level)
        {
            _levelLabel.text = (level + 1).ToString();
            _nextLevelLabel.text = (level + 2).ToString();
        }
    }
}
