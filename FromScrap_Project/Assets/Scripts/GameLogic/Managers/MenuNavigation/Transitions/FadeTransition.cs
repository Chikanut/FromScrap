using System;
using DG.Tweening;
using UnityEngine;

namespace MenuNavigation
{
    public class FadeTransition : ShowableTransitionBase
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private float _duration = 0.5f;

        private Sequence _showSequence;

        public override void Show(Action onFinish)
        {
            _showSequence?.Kill();
            _showSequence = DOTween.Sequence();
            _group.alpha = 0;

            _showSequence.SetUpdate(true);
            _showSequence.Insert(0,
                _group.DOFade(1, _duration)
            );

            _showSequence.OnComplete(() => { onFinish?.Invoke(); });
            _showSequence.Play();
        }

        public override void Hide(Action onFinish)
        {

            _showSequence?.Kill();
            _showSequence = DOTween.Sequence();
            _group.alpha = 1;
            _showSequence.SetUpdate(true);
            _showSequence.Insert(0,
                _group.DOFade(0, _duration)
            );

            _showSequence.onComplete = () => { onFinish?.Invoke(); };

            _showSequence.Play();
        }
    }
}
