using System;
using System.Collections.Generic;
using UnityEngine;

namespace MenuNavigation {
    public abstract class MenuScreen : Showable
    {
        public override bool IsActive {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
        
        public override ShowableTransitionState TransitionState { get; protected set; }
    }
}