using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ShootCommon.Views.Mediation;
using UnityEngine;

namespace MenuNavigation {
    public abstract class Showable : View {
        
        public ShowableTransitionBase TransitionBase;
        
        public static string ShowableName = "ScreenName";
        [HideInInspector] public string CurrentShowableName;

        protected MenuNavigationController MenuNavigationController;

        public static async Task<T> Create<T>(MenuNavigationController menuNavigationController, Transform parent, [CanBeNull] string showableName = null) where T : Showable
        {
            showableName ??= typeof(T).GetField("ShowableName").GetValue(null).ToString();

            var asyncOperation = Resources.LoadAsync(showableName);

            while (!asyncOperation.isDone)
                await Task.Yield();

            var screen = (Instantiate(asyncOperation.asset, parent) as GameObject)?.GetComponent<T>();
            
            if (screen != null)
            {
                screen.MenuNavigationController = menuNavigationController;
                screen.CurrentShowableName = showableName;

                return screen;
            }
            
            Debug.LogError("Screen that you want to create - " + typeof(T) + " cant be created, check ShowableName parameter");

            return null;
        }

        public virtual void Show(Action onFinish)
        {
            if (TransitionBase != null)
            {
                TransitionState = ShowableTransitionState.Showing;
                TransitionBase.Show(() =>
                {
                    TransitionState = ShowableTransitionState.Shown;
                    onFinish?.Invoke();
                });
            }
            else
            {
                TransitionState = ShowableTransitionState.Shown;
                onFinish?.Invoke();
            }
        }

        public virtual void Hide(Action onFinish)
        {
            if (TransitionBase != null)
            {
                TransitionState = ShowableTransitionState.Hiding;
                TransitionBase.Hide(() =>
                {
                    TransitionState = ShowableTransitionState.Hidden;
                    IsActive = false;
                    onFinish?.Invoke();
                });
            }
            else
            {
                TransitionState = ShowableTransitionState.Hidden;
                IsActive = false;
                onFinish?.Invoke();
            }
        }

        public abstract bool IsActive { get; set; }
        public abstract ShowableTransitionState TransitionState { get; protected set; }
    }

    public enum ShowableTransitionState {
        None,
        Showing,
        Shown,
        Hiding,
        Hidden,
    }
}