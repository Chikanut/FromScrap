using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class LineLayout : MonoBehaviour
{
    public List<RectTransform> _anchors = new List<RectTransform>();

    private RectTransform _rectTransform;

    private RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }
    private readonly List<RectTransform> _children = new List<RectTransform>();
    
    private void OnEnable()
    {
        UpdateChildrens();
    }
    
    private void OnTransformChildrenChanged()
    {
        UpdateChildrens();
    }

    void UpdateChildrens()
    {
        _children.Clear();
        
        for(int i = 0 ; i < RectTransform.childCount ; i ++)
        {
            if(RectTransform.GetChild(i).GetComponent<RectTransform>() != null)
            {
                var rectTrans = RectTransform.GetChild(i).GetComponent<RectTransform>();
                
                if(!_anchors.Contains(rectTrans))
                    _children.Add(rectTrans);
            }
        }
    }

    private void Update()
    {
        if (_anchors.Count > 1)
        {
            var distance = 0f;

            for (int i = 1; i < _anchors.Count; i++)
            {
                distance += Vector2.Distance(_anchors[i].localPosition, _anchors[i - 1].localPosition);
            }
            
            var step = distance / (_children.Count -1);
            
            var currentDistance = 0f;
            
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].localPosition = GetPositionOnAnchors(currentDistance);
                currentDistance += step;
            }
        }
        else
        {
            _children.ForEach(ch=>ch.localPosition = Vector2.zero);
        }
    }

    Vector2 GetPositionOnAnchors(float distance)
    {
        var pos = _anchors[0].localPosition;
        var i = 0;
        while(distance > 0)
        {
            var nextPos = _anchors[i].localPosition;
            
            if(_anchors.Count > i+1)
            {
                nextPos = _anchors[i+1].localPosition;
            }
            else
            {
                distance = 0;
                pos = nextPos;
                break;
            }

            var nextDistance = Vector2.Distance(pos, nextPos);
            
            if(distance > nextDistance)
            {
                distance -= nextDistance;
                pos = nextPos;
            }
            else
            {
                var dir = (nextPos - pos).normalized;
                return pos + dir * distance;
            }

            i++;
        }

        return pos;
    }

    private void OnDrawGizmosSelected()
    {
        if (_anchors.Count > 1)
        {
            Gizmos.color = Color.green;
            
            for (int i = 1; i < _anchors.Count; i++)
            {
                Gizmos.DrawLine(_anchors[i - 1].position, _anchors[i].position);    
            }
        }
    }
}
