using System;
using UnityEngine;

namespace MenuNavigation
{
    public abstract class ShowableTransitionBase : MonoBehaviour
    {
        public abstract void Show(Action onFinish);
        public abstract void Hide(Action onFinish);
    }
}
