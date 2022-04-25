using ArtResources.TrailEffectArt.Scripts;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class GameObjectTrackEntityComponent : IComponentData
{
    public GameObject TrackingRefGO;
    public bool IsPointToPointTrack;
    public float TrackSpeed;

    public GameObject _currentTrackingGO;
    public bool _isNewInstance = false;

    public GameObjectTrackEntityComponent Init(GameObject trackingRefGO, bool isPointToPointTrack = true, float trackSpeed = 2f)
    {
        TrackingRefGO = trackingRefGO;
        IsPointToPointTrack = isPointToPointTrack;
        TrackSpeed = trackSpeed;

        return this;
    }

    public void UpdateTrackingGOPosition(Vector3 pos)
    {
        if (_currentTrackingGO == null)
            return;

        if (!_isNewInstance)
            return;

        if (IsPointToPointTrack)
            _currentTrackingGO.transform.position = pos;
        else
            _currentTrackingGO.transform.position =
                Vector3.Lerp(_currentTrackingGO.transform.position, pos, TrackSpeed * Time.deltaTime);
    }

    public void InitTrackingGO(bool IsNewInstance)
    {
        if (TrackingRefGO == null)
            return;

        if (IsNewInstance)
        {
            if (!_isNewInstance)
            {
                _currentTrackingGO = Object.Instantiate(TrackingRefGO);
                _currentTrackingGO.GetComponent<TrailEffectTrackingController>().Init();
                _isNewInstance = true;
            }
        }
        else if(_currentTrackingGO != null)
        {
            _currentTrackingGO.GetComponent<TrailEffectTrackingController>().Remove();
            _currentTrackingGO = null;
            _isNewInstance = false;
        }
    }
}