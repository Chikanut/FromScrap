using UnityEngine;
using UnityEditor;

namespace Visartech.Progress.Editor
{
    public class ClearProgressTab
    {
        [MenuItem("Tools/Clear all Saves")]
        static void deleteAllExample()
        {
            if (EditorUtility.DisplayDialog("Delete all saves.",
                    "Are you sure you want to delete all player prefs? " +
                    "This action cannot be undone.", "Yes", "No"))
            {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}