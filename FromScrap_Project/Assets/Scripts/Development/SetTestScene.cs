using ShootCommon.GlobalStateMachine;
using UnityEngine;
using Visartech.Progress;

public class SetTestScene : MonoBehaviour
{
    public StateMachineTriggers TestState;
    
    private void Awake()
    {
        Progress.Development.isTesting = true;
        Progress.Development.testState = TestState;
    }
}
