using UnityEngine;
using UnityEngine.SceneManagement;
using Visartech.Progress;

public class SetTestScene : MonoBehaviour
{
    // public string TestScene;
    
    private void Awake() 
    {
        Progress.Development.isTesting = true;
        Progress.Development.testSceneName = SceneManager.GetActiveScene().name;
    }
}
