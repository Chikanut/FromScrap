using UnityEngine;
using UnityEngine.SceneManagement;
using Visartech.Progress;

public class SetTestScene : MonoBehaviour
{
    public static bool isInitialized = false;
    
    private void Awake()
    {
        if (isInitialized) return;
        
        Progress.Development.isTesting = true;
        Progress.Development.testSceneName = SceneManager.GetActiveScene().name;
        isInitialized = true;
    }
}
