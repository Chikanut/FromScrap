using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TemplateData))]
public class TemplateDataEditor : Editor
{
    private SerializedProperty _templateData;
    int _currentTab;
    
    private void OnEnable()
    {
        _templateData = serializedObject.FindProperty("TemplateFiles");
        InitCreationData();

        if (serializedObject.FindProperty("_isCreation").boolValue)
            _currentTab = 1;
    }
    
    public override void OnInspectorGUI()
    {
        _currentTab = GUILayout.Toolbar (_currentTab, new string[] {"Data", "Create"});
        serializedObject.FindProperty("_isCreation").boolValue = _currentTab == 1;
        switch (_currentTab) {
            case 0 :
                DrawData();
                _creationDataInited = false;
                break;
            case 1:
                DrawCreate();
                break;
        }
        
        serializedObject.ApplyModifiedProperties ();
    }

    void DrawData()
    {
        HorizontalLine( Color.grey );

        for (int i = 0; i < _templateData.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            
            _templateData.GetArrayElementAtIndex(i).FindPropertyRelative("FileNameTemplate").stringValue =
                EditorGUILayout.TextField("File name template : ",
                    _templateData.GetArrayElementAtIndex(i).FindPropertyRelative("FileNameTemplate").stringValue);

            _templateData.GetArrayElementAtIndex(i).FindPropertyRelative("FileNameReplace").stringValue =
                EditorGUILayout.TextField("File name replace : ",
                    _templateData.GetArrayElementAtIndex(i).FindPropertyRelative("FileNameReplace").stringValue);
            
            EditorGUILayout.EndVertical();
            
            if (GUILayout.Button("-"))
                _templateData.DeleteArrayElementAtIndex(i);

            EditorGUILayout.EndHorizontal();
            
            
            EditorGUILayout.LabelField("Template : ");
            _templateData.GetArrayElementAtIndex(i).FindPropertyRelative("Template").stringValue =
                EditorGUILayout.TextArea(_templateData.GetArrayElementAtIndex(i).FindPropertyRelative("Template")
                    .stringValue);
            
            EditorGUILayout.LabelField("Replace names : ");
            EditorGUILayout.BeginHorizontal();

            var replaceNames = _templateData.GetArrayElementAtIndex(i).FindPropertyRelative("ReplaceNames");
            
            for (int j = 0; j < replaceNames.arraySize; j++)
            {          
                EditorGUILayout.BeginVertical();
                replaceNames.GetArrayElementAtIndex(j).stringValue = EditorGUILayout.TextField(replaceNames.GetArrayElementAtIndex(j).stringValue);
                if (GUILayout.Button("-"))
                    replaceNames.DeleteArrayElementAtIndex(j);
                EditorGUILayout.EndVertical();
            }
  
            if (GUILayout.Button("+"))
            {
                replaceNames.arraySize++;
            }
            EditorGUILayout.EndHorizontal();
            
            HorizontalLine( Color.grey );
        }

        GUILayout.Space(15);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+"))
        {
            _templateData.arraySize++;
        }
        EditorGUILayout.EndHorizontal();
    }

    private bool _creationDataInited;
    private string _templateName;
    private List<List<string>> _replaceTexts = new List<List<string>>();

    public class FileCreationData
    {
        public string FileName;
        public string FileText;
    }

    private List<FileCreationData> _filesData = new List<FileCreationData>();

    void InitCreationData()
    {
        _creationDataInited = true;

        _templateName = "";
        _replaceTexts = new List<List<string>>();
        _filesData =  new List<FileCreationData>();
        
        var templateData = (target as TemplateData)?.TemplateFiles;

        for (int i = 0; i < templateData.Count; i++)
        {
            _replaceTexts.Add(new List<string>());
            _filesData.Add(new FileCreationData());
            for (int j = 0; j < templateData[i].ReplaceNames.Count; j++)
            {
                _replaceTexts[i].Add("");
            }
        }
    }

    void DrawCreate()
    {
        if (!_creationDataInited)
        {
            InitCreationData();
        }

        HorizontalLine( Color.grey );
        
        var templateData = (target as TemplateData)?.TemplateFiles;

        _templateName = EditorGUILayout.TextField("Template name : ", _templateName);
        
        for (int i = 0; i < templateData.Count; i++)
        {
            _filesData[i].FileName = templateData[i].FileNameTemplate
                .Replace(templateData[i].FileNameReplace, _templateName);
            
            EditorGUILayout.LabelField(_filesData[i].FileName+".cs");
            
            _filesData[i].FileText = templateData[i].Template;
            _filesData[i].FileText =  _filesData[i].FileText.Replace("*Name", _templateName);
            for (int j = 0; j < templateData[i].ReplaceNames.Count; j++)
            {
                _filesData[i].FileText =  _filesData[i].FileText.Replace(templateData[i].ReplaceNames[j], _replaceTexts[i][j]);
            }

            EditorGUILayout.TextArea( _filesData[i].FileText);

            for (int j = 0; j < templateData[i].ReplaceNames.Count; j++)
            {
                _replaceTexts[i][j] =
                    EditorGUILayout.TextField(templateData[i].ReplaceNames[j], _replaceTexts[i][j]);
            }
            
            HorizontalLine( Color.grey );
        }

        if (GUILayout.Button("Create"))
        {
            var path = EditorUtility.OpenFolderPanel("Save template", "Assets/Scripts/GameLogic", "");
            path += "/";
            for (int i = 0; i < _filesData.Count; i++)
            {
                var filePath = path + _filesData[i].FileName + ".cs";

                System.IO.File.WriteAllText(filePath, _filesData[i].FileText);
            }
            
            AssetDatabase.Refresh();
        }
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
