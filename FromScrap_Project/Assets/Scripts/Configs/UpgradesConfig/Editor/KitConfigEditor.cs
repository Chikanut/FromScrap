using System.Collections.Generic;
using EditorTools.Editor;
using I2.Loc;
using Packages.Common.Storage.Config.Upgrades;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(KitConfigScriptable))]
public class KitConfigEditor : Editor
{
    private ReorderableList _kitsList;
    private SerializedProperty _data;

    public void OnEnable()
    {
        _data = serializedObject.FindProperty("Data");
        _kitsList = new ReorderableList(serializedObject, _data.FindPropertyRelative("KitObjects"), true, true, true, true);
        _kitsList.drawElementCallback = DrawElementCallback;
        _kitsList.elementHeightCallback = ElementHeightCallback;
    }

    private float ElementHeightCallback(int index)
    {
        var arraySize = _kitsList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("Descriptions")
            .arraySize;
        
        return EditorGUIUtility.singleLineHeight * 3 +
               arraySize * EditorGUIUtility.singleLineHeight * 2 + 11 *arraySize;
    }

    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _kitsList.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight/2, 200, EditorGUIUtility.singleLineHeight), 
            element.FindPropertyRelative("Authoring"),
            GUIContent.none
        );
        
        DrawDescriptions(element, rect);
    }
    
    void DrawDescriptions(SerializedProperty element, Rect rect)
    {
        var descriptionsList = element.FindPropertyRelative("Descriptions");

        var currentHeight = rect.y + EditorGUIUtility.singleLineHeight * 1.5f;
        
        for (int d = 0; d < descriptionsList.arraySize; d++)
        {
            var description = descriptionsList.GetArrayElementAtIndex(d);
            
            var currentPos = rect.x;
            currentHeight += 3;
            
            EditorGUI.LabelField(new Rect(rect.x-10, currentHeight, 70, EditorGUIUtility.singleLineHeight), "Description");

            description.FindPropertyRelative("DescriptionKey").stringValue = EditorExtensions.GetLocalizationTermField(new Rect(currentPos + 65, currentHeight, 140, EditorGUIUtility.singleLineHeight), description.FindPropertyRelative("DescriptionKey").stringValue);
            
            
            currentPos += 210;
            
            for (int i = 0; i < description.FindPropertyRelative("Values").arraySize; i++)
            {
                var value = description.FindPropertyRelative("Values").GetArrayElementAtIndex(i);
            
                EditorGUI.PropertyField(
                    new Rect(currentPos, currentHeight, 25, EditorGUIUtility.singleLineHeight), 
                    value,
                    GUIContent.none
                );
            
                if(GUI.Button(new Rect(currentPos + 30, currentHeight, 25, EditorGUIUtility.singleLineHeight), "-"))
                {
                    description.FindPropertyRelative("Values").DeleteArrayElementAtIndex(i);
                }
            
                currentPos += 60;
            }
     
        
            if(GUI.Button(new Rect(currentPos, currentHeight, 50, EditorGUIUtility.singleLineHeight), "+"))
            {
                description.FindPropertyRelative("Values").arraySize++;
            }
            
            currentPos += 60;
            
            var arguments = description.FindPropertyRelative("Values").arraySize;
            var descriptionText = LocalizationManager.GetTranslation(description.FindPropertyRelative("DescriptionKey").stringValue);

            if (!string.IsNullOrEmpty(descriptionText))
            {
                for (var i = 0; i < arguments; i++)
                {
                    if (descriptionText.Contains("{" + i + "}"))
                    {
                        descriptionText = descriptionText.Replace("{" + i + "}",
                            description.FindPropertyRelative("Values").GetArrayElementAtIndex(i).floatValue.ToString());
                    }
                }
            }

            EditorGUI.LabelField(new Rect(currentPos, currentHeight, 500, EditorGUIUtility.singleLineHeight), descriptionText);
            
            currentHeight += 3;
            
            // EditorGUI.LabelField(new Rect(rect.x + 100,currentHeight + EditorGUIUtility.singleLineHeight,50,EditorGUIUtility.singleLineHeight), "Remove");
            if(GUI.Button(new Rect(rect.x + 200-50, currentHeight + EditorGUIUtility.singleLineHeight, 50, EditorGUIUtility.singleLineHeight), "-"))
            {
                element.FindPropertyRelative("Descriptions").DeleteArrayElementAtIndex(d);
            }
            
            EditorGUI.DrawRect(new Rect(rect.x, currentHeight + EditorGUIUtility.singleLineHeight * 2f + 3, EditorGUIUtility.currentViewWidth-50, 1), new Color ( 0.5f,0.5f,0.5f, 1 ) );
            currentHeight += EditorGUIUtility.singleLineHeight * 2 + 5;
        }

        currentHeight += 5;
        
        if(GUI.Button(new Rect(rect.x, currentHeight, 50, EditorGUIUtility.singleLineHeight), "+"))
        {
            element.FindPropertyRelative("Descriptions").arraySize++;
        }
    }

    public override void OnInspectorGUI()
    {
        UpdateKitsPrefabsInfo();
        
        serializedObject.Update();

        _data.FindPropertyRelative("ID").stringValue = EditorGUILayout.TextField("ID", _data.FindPropertyRelative("ID").stringValue);
        _data.FindPropertyRelative("NameLocKey").stringValue = EditorExtensions.GetLocalizationTermFieldLayout(_data.FindPropertyRelative("NameLocKey").stringValue, "Name Localization Key");
        _data.FindPropertyRelative("DescriptionLocKey").stringValue = EditorExtensions.GetLocalizationTermFieldLayout(_data.FindPropertyRelative("DescriptionLocKey").stringValue, "Description Localization Key");
        
        // _data.FindPropertyRelative("NameLocKey").stringValue = EditorGUILayout.TextField("Name Localization Key", _data.FindPropertyRelative("NameLocKey").stringValue);
        // _data.FindPropertyRelative("DescriptionLocKey").stringValue = EditorGUILayout.TextField("Description Localization Key", _data.FindPropertyRelative("DescriptionLocKey").stringValue);
        EditorGUILayout.PropertyField(_data.FindPropertyRelative("Icon"));
        EditorGUILayout.PropertyField(_data.FindPropertyRelative("Type"));
        EditorGUILayout.PropertyField(_data.FindPropertyRelative("IsStacked"));
        EditorGUILayout.PropertyField(_data.FindPropertyRelative("isDefault"));
        
        _kitsList.DoLayoutList();
        
        serializedObject.ApplyModifiedProperties();


    }

    void UpdateKitsPrefabsInfo()
    {
        var baseInfo = (target as KitConfigScriptable);
        var kitsInfo = baseInfo.Data.KitObjects;

        for (int i = 0; i < kitsInfo.Count; i++)
        {
            var changed = false;
            
            if(kitsInfo[i].Authoring == null)
                return;

            for (int j = 0; j < kitsInfo.Count; j++)
            {
                if(kitsInfo[j].Authoring == kitsInfo[i].Authoring && j != i)
                    return;
            }

            if (kitsInfo[i].Authoring.Type != baseInfo.Data.Type)
            {
                kitsInfo[i].Authoring.Type = baseInfo.Data.Type;
                changed = true;
            }

            if (kitsInfo[i].Authoring.IsStacked != baseInfo.Data.IsStacked)
            {
                kitsInfo[i].Authoring.IsStacked = baseInfo.Data.IsStacked;
                changed = true;
            }
            
            if (kitsInfo[i].Authoring.KitLevel != i)
            {
                kitsInfo[i].Authoring.KitLevel = i;
                changed = true;
            }
            
            if (changed)
                PrefabUtility.SavePrefabAsset(kitsInfo[i].Authoring.gameObject);
        }
    }
}
