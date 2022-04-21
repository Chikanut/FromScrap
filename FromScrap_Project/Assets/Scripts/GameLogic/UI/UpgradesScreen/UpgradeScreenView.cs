using System;
using DG.Tweening;
using MenuNavigation;
using Unity.Entities;

namespace UI.Upgrades
{
    public class UpgradeScreenView : MenuScreen
    {
        private Action _onComplete;

        public void Init(Entity target, int level, Action onComplete)
        {
            _onComplete = onComplete;


        }

        void Complete()
        {
            _onComplete?.Invoke();
            MenuNavigationController.HideMenuScreen(this);
        }

    }
}