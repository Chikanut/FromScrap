using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TemplateCreationWindow : EditorWindow
{
    [MenuItem("Tools/Templates/Create Template")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TemplateCreationWindow));
    }

    private string _templateName;
    List<TemplateData.TemplateFile> _templateFiles = new List<TemplateData.TemplateFile>();
    
    void OnGUI()
    {
        HorizontalLine( Color.grey );
        EditorGUILayout.BeginHorizontal();
        _templateName = EditorGUILayout.TextField ("Template name : ", _templateName);
        if (GUILayout.Button("Save Template"))
        {
            var data = CreateInstance(typeof(TemplateData)) as TemplateData;

            if (data != null)
            {
                data.TemplateFiles = _templateFiles;

                var path = EditorUtility.SaveFilePanelInProject("Save template", _templateName, "asset", "",
                    "Assets/Templates");
                
                if (path.Length != 0)
                {
                    AssetDatabase.CreateAsset(data, path);
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = data;

                    Close();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        HorizontalLine( Color.grey );

        for (int i = 0; i < _templateFiles.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            _templateFiles[i].FileNameTemplate = EditorGUILayout.TextField ("File name template : ", _templateFiles[i].FileNameTemplate);
            _templateFiles[i].FileNameReplace = EditorGUILayout.TextField ("File name replace : ", _templateFiles[i].FileNameReplace);
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("-"))
            {
                _templateFiles.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
            
            
            EditorGUILayout.LabelField("Template : ");
            _templateFiles[i].Template = EditorGUILayout.TextArea(_templateFiles[i].Template);
            
            EditorGUILayout.LabelField("Replace names : ");
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < _templateFiles[i].ReplaceNames.Count; j++)
            {          
                EditorGUILayout.BeginVertical();
                _templateFiles[i].ReplaceNames[j] = EditorGUILayout.TextField(_templateFiles[i].ReplaceNames[j]);
                if (GUILayout.Button("-"))
                {
                    _templateFiles[i].ReplaceNames.RemoveAt(j);
                }
                EditorGUILayout.EndVertical();
            }
  
            if (GUILayout.Button("+"))
            {
                _templateFiles[i].ReplaceNames.Add("");
            }
            EditorGUILayout.EndHorizontal();
            
            HorizontalLine( Color.grey );
        }

        GUILayout.Space(15);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            _templateFiles.Add(new TemplateData.TemplateFile());
        }
        EditorGUILayout.EndHorizontal();
    }
    
    
    static void HorizontalLine ( Color color ) {
        
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset( 0, 0, 4, 4 );
        horizontalLine.fixedHeight = 1;
        
        GUILayout.Space(5);
        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box( GUIContent.none, horizontalLine );
        GUI.color = c;
        GUILayout.Space(5);
    }
}
