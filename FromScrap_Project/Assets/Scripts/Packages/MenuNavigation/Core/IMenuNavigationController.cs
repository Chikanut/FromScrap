using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MenuNavigation {
    public interface IMenuNavigationController {
        Task<T> ShowMenuScreen<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : MenuScreen;
        void HideMenuScreen<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : MenuScreen;
        void HideMenuScreen(MenuScreen menuScreen, Action onFinish = null);
        Task<T> ShowPopup<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : Popup;
        void HidePopup<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : Popup;
        void HidePopup(Popup popup, Action onFinish = null);
        void HideAllPopups();
        bool TryGetMenuScreen<T>(out T menuScreen, [CanBeNull] string showableName = null) where T : MenuScreen;
        bool TryGetPopup<T>(out T popup, [CanBeNull] string showableName = null) where T : Popup;
        bool Interactable { get; set; }
    }
}
