using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageTextPoolObject : PoolObject
{
    [Header("Components")]
    [SerializeField] private TextMeshPro _text;

    [Header("Animation")]
    [SerializeField] private float _travelUpDistance;
    [SerializeField] private float _scaleDelta;
    [Range(0,0.5f)]
    [SerializeField] private float _appearRatio = 0.10f;
    [SerializeField] private float _animationTime;

    Vector3 _startScale = Vector3.one;
    
    private void Awake()
    {
        _startScale = transform.localScale;
    }

    public void SetText(string text)
    {
        _text.text = text;

        PlayAnim();
    }

    private Sequence _sequence;
    
    void PlayAnim()
    {
        _sequence?.Kill();
        transform.localScale = _startScale;
        _text.alpha = 0;
        _sequence = DOTween.Sequence();

        _sequence.Insert(0, transform.DOMove(transform.position + Vector3.up * _travelUpDistance, _animationTime));
        _sequence.Insert(0, transform.DOScale(_scaleDelta * transform.localScale, _animationTime));
        _sequence.Insert(0, _text.DOFade(1, _animationTime * _appearRatio));
        _sequence.Insert(_animationTime * (1-_appearRatio), _text.DOFade(0, _animationTime * _appearRatio));
        
        _sequence.onComplete = Destroy;
    }

    public override void AcceptObjectsLinks(List<PoolObject> objects)
    {
        
    }

    public override void ResetState()
    {
   
    }
}
