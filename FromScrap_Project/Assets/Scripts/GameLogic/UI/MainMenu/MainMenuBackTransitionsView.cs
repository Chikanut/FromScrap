using System.Collections.Generic;
using System.Linq;
using ShootCommon.Views.Mediation;
using UnityEngine;

public class MainMenuBackTransitionsView : View
{
    [System.Serializable]
    public class CameraState
    {
        public string StateName;
        public string TriggerName;
    }

    public Transform CarPosition;
    [SerializeField] private Animator _camerasBlend;
    [SerializeField] private List<CameraState> _cameraStates = new List<CameraState>();
    
    public void OnStateChanged(string state)
    {
        foreach (var cameraState in _cameraStates.Where(cameraState => cameraState.StateName == state))
        {
            _camerasBlend.SetTrigger(cameraState.TriggerName);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        Debug.LogError("View enabled");
    }
}
