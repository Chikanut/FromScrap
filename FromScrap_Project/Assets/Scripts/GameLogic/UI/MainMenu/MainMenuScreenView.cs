using System.Collections.Generic;
using DG.Tweening;
using MenuNavigation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.MainMenu
{
    public class MainMenuScreenView : MenuScreen
    {
        [System.Serializable]
        public class TabInfo
        {
            public MainMenuToggle Toggle;
            public MainMenuTab Tab;
        }

        [Header("PlayerInfo")]
        [SerializeField] private TextMeshProUGUI _levelNum;
        [SerializeField] private Slider _levelProgress;
        [SerializeField] private TextMeshProUGUI _scrapCount;
        
        [Header("Tabs")]
        [SerializeField] private List<TabInfo> _mainMenuStructure = new List<TabInfo>();
        [SerializeField] int _defaultTabIndex = 0;

        [Header("Test")]
        public int StartLevel = 0;

        public int StartScrap = 0;
        

        private void Awake()
        {
            InitToggles();
        }

        void InitToggles()
        {
            foreach (var tabInfo in _mainMenuStructure)
            {
                tabInfo.Toggle.Init();
                tabInfo.Toggle.OnActive = (b) =>
                {
                    if (tabInfo.Tab == null) return;
                    
                    if (b) tabInfo.Tab.Show();
                    else tabInfo.Tab.Hide();
                };
                
                if (tabInfo.Tab == null) continue;
                    tabInfo.Tab.Hide();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            CheckNew();
            
            _mainMenuStructure[_defaultTabIndex].Toggle.Enable();
        }

        public void CheckNew()
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                foreach (var tabInfo in _mainMenuStructure)
                {
                    if (tabInfo.Tab == null) return;
                    tabInfo.Toggle.SetNew(tabInfo.Tab.HasNew);
                }
            });
        }

        public void InitPlayerInfo(int levelNum, float levelProgress, int scrapCount)
        {
            _levelNum.text = levelNum.ToString();
            _levelProgress.value = levelProgress;
            _scrapCount.text = scrapCount.ToString();
        }
    }
}