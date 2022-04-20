using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace MenuNavigation {
    public class MenuNavigationController : IMenuNavigationController, IInitializable
    {
        private MenuNavigationControllerCanvas _canvas;

        private readonly Dictionary<string, Showable> _popups = new Dictionary<string, Showable>();
        private readonly Dictionary<string, Showable> _menuScreens = new Dictionary<string, Showable>();

        private readonly Dictionary<string, List<ScreenElement>> _screenElementsPool =
            new Dictionary<string, List<ScreenElement>>();
        
        public static MenuNavigationController Instance;
        
        [Inject]
        public void Init()
        {
            _canvas = Object.Instantiate(Resources.Load("MenuNavigationCanvas") as GameObject).GetComponent<MenuNavigationControllerCanvas>();
            Instance = this;
        }
        
        public void Initialize() { }

        public async Task<T> ShowMenuScreen<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : MenuScreen
        {
            return await ShowShowable<T>(_menuScreens, _canvas.MenuScreensParent, onFinish, showableName);
        }
        
        public void HideMenuScreen<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : MenuScreen
        {
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();
            
            _canvas.CanvasGroup.interactable = false;
            HideAllPopups();
            HideAllScreensElements();

            HideShowable<T>(_menuScreens, () =>
            {
                _canvas.CanvasGroup.interactable = true;
                onFinish?.Invoke();
            }, showableName);
        }

        public void HideMenuScreen(MenuScreen menuScreen, Action onFinish = null)
        {
            _canvas.CanvasGroup.interactable = false;
            HideAllPopups();
            HideAllScreensElements();
            menuScreen.Hide(() =>
            {
                _canvas.CanvasGroup.interactable = true;
                onFinish?.Invoke();
            });
        }

        public async Task<T> ShowPopup<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : Popup
        {
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();
            return await ShowShowable<T>(_popups,  _canvas.PopupsParent, onFinish, showableName);
        }
        
        public void HidePopup<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : Popup
        {
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();
            _canvas.CanvasGroup.interactable = false;

            HideShowable<T>(_popups,
                () =>
                {
                    _canvas.CanvasGroup.interactable = true;
                    onFinish?.Invoke();
                }, showableName);
        }

        public void HidePopup(Popup popup, Action onFinish = null)
        {
            _canvas.CanvasGroup.interactable = false;
            popup.Hide(() =>
            {
                _canvas.CanvasGroup.interactable = true;
                onFinish?.Invoke();
            });
        }

        public void HideAllPopups()
        {
            foreach (var popup in _popups.Values)
                popup.IsActive = false;
        }

        public async Task<T> ShowScreenElement<T>(Action onFinish = null, [CanBeNull] string showableName = null) where T : ScreenElement
        {
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();
            _canvas.CanvasGroup.interactable = false;

            T element = null;
            var type = typeof(T);

            foreach (var elementsPool in _screenElementsPool.Where(elementsPool => elementsPool.Key == showableName))
            {
                foreach (var elements in elementsPool.Value.Where(elements => !elements.IsActive))
                {
                    element = (T) elements;
                    break;
                }

                break;
            }
            
            if (element == null)
            {
                element = await Showable.Create<T>(this,  _canvas.ScreenElementsPoolParent, showableName);

                if (_screenElementsPool.ContainsKey(showableName))
                    _screenElementsPool[showableName].Add(element);
                else
                    _screenElementsPool.Add(showableName, new List<ScreenElement> {element});
            }
            
            element.IsActive = true;
            
            element.Show(() =>
            {
                _canvas.CanvasGroup.interactable = true;
                onFinish?.Invoke();
            });

            return element;
        }

        public void HideScreenElement(ScreenElement element, Action onFinish = null)
        {
            _canvas.CanvasGroup.interactable = false;
            element.Hide(() =>
            {
                _canvas.CanvasGroup.interactable = true;
                element.IsActive = false;
                onFinish?.Invoke();
            });
        }

        public void HideAllScreensElements()
        {
            foreach (var elements in _screenElementsPool.SelectMany(elementsPool => elementsPool.Value))
            {
                elements.IsActive = false;
                elements.transform.SetParent( _canvas.ScreenElementsPoolParent);
            }
        }

        public void ReturnScreenElement(ScreenElement element)
        {
            element.transform.SetParent(_canvas.ScreenElementsPoolParent);
        }

        private async Task<T> ShowShowable<T>(Dictionary<string, Showable> pool, Transform parent, Action onFinish = null, [CanBeNull] string showableName = null) where T : Showable
        { 
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();

            _canvas.CanvasGroup.interactable = false;
            
            if (!TryGetShowable<T>(out var showable, pool, ShowableSearchMathod.onlyDisabled, showableName))
                showable = await Showable.Create<T>(this, parent, showableName);

            pool[showableName] = showable;
            showable.transform.SetAsLastSibling();
            showable.IsActive = true;
            showable.Show(() =>
            {
                _canvas.CanvasGroup.interactable = true;
                onFinish?.Invoke();
            });
            
            return showable;
        }

        private enum ShowableSearchMathod
        {
            any,
            onlyDisabled,
            onlyEnabled
        }

        private bool TryGetShowable<T>(out T showable, Dictionary<string, Showable> pool, ShowableSearchMathod searchMethod, [CanBeNull] string showableName = null) where T : Showable
        {
            var sortedShowables = pool.Where(m =>
                (showableName == null || m.Key == showableName) &&
                (searchMethod != ShowableSearchMathod.onlyDisabled || !m.Value.IsActive) &&
                (searchMethod != ShowableSearchMathod.onlyEnabled || m.Value.IsActive)).ToList();
            
            var hasShowable = sortedShowables.Count > 0;

            showable = hasShowable ? sortedShowables[0].Value as T : null;

            return hasShowable;
        }
        
        private void HideShowable<T>(Dictionary<string, Showable> pool, Action onFinish = null, [CanBeNull] string showableName = null) where T : Showable
        {
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();
            
            if (!pool.Values.Any(e => e.IsActive))
            {
                onFinish?.Invoke();
                return;
            }

            if (!TryGetShowable<T>(out var screen, pool, ShowableSearchMathod.onlyEnabled, showableName))
            {
                Debug.LogWarning($"Menu Screen of type '{typeof(T)}' does not exist yet!");
                return;
            }
            
            screen.Hide(onFinish);
        }

        public bool Interactable
        {
            get =>  _canvas.CanvasGroup.interactable;
            set =>  _canvas.CanvasGroup.interactable = value;
        }
    }
}