using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TemplateApplyWindow : EditorWindow
{
    [MenuItem("Tools/Templates/Apply Template")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TemplateApplyWindow));
    }
    
    List<TemplateData.TemplateFile> _templateInfo = new List<TemplateData.TemplateFile>();
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("Select template : ");
        
        var instances = GetAllInstances<TemplateData>();

        for (int i = 0; i < instances.Length; i++)
        {
            if (GUILayout.Button(instances[i].name))
            {
                EditorUtility.FocusProjectWindow();

                instances[i]._isCreation = true;
                Selection.activeObject = instances[i];
                
                Close();
            }
        }
    }
    
    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for(int i =0;i<guids.Length;i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }
 
        return a;
 
    }
    
}
