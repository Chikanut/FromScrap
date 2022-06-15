using System;
using MenuNavigation;
using Packages.Common.Storage.Config.Upgrades;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PopUps.Pause
{
    public class PausePopUpView : Popup
    {
        [Header("Pause PopUp")]
        [SerializeField] private Button _continue;
        [SerializeField] private Button _mainMenu;
        [SerializeField] private Button _restart;
        
        [SerializeField] CharacteristicsPanel _characteristicsPanel;
        [SerializeField] private UpgradesInfoPanelChess _upgradesPanel;
        
        public Action OnContinueAction;
        public Action OnMainMenuAction;
        public Action OnRestartAction;

        protected override void Start()
        {
            base.Start();

            _continue.onClick.AddListener(OnContinue);
            _mainMenu.onClick.AddListener(OnStartGame);
            _restart.onClick.AddListener(OnExitGame);
        }
        
        public void UpdateInfo(CurrentCarInfoData carInfo, UpgradesConfigData upgradesConfigData)
        {
            _upgradesPanel.UpdateInfo(carInfo, upgradesConfigData);
            _characteristicsPanel.UpdateCharacteristics(carInfo.carData.BaseCharacteristics,
                carInfo.CurrentCharacteristics);
            CarViewCamera.Instance.ShowCar(carInfo.carEntity);
        }

        void OnContinue()
        {
            OnContinueAction?.Invoke();
        }

        void OnStartGame()
        {
            OnMainMenuAction?.Invoke();
        }

        void OnExitGame()
        {
            OnRestartAction?.Invoke();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Time.timeScale = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Time.timeScale = 1;
            CarViewCamera.Instance.Hide();
        }
    }
}