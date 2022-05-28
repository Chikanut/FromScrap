using UnityEngine;
using UnityEditor;

namespace Visartech.Progress.Editor
{
    public class ClearProgressTab
    {
        [MenuItem("Tools/EditorPrefs/Clear all Editor Preferences")]
        static void deleteAllExample()
        {
            if (EditorUtility.DisplayDialog("Delete all editor preferences.",
                    "Are you sure you want to delete all the editor preferences? " +
                    "This action cannot be undone.", "Yes", "No"))
            {
                EditorPrefs.DeleteAll();
            }
        }
    }
}