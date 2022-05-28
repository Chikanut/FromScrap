using EditorTools.Editor;
using I2.Loc;
using Packages.Common.Storage.Config;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(PlayerUpgradeScriptable))]
public class PlayerUpgradeScriptableEditor : Editor
{
    private ReorderableList _upgradesList;
    private SerializedProperty _data;

    public void OnEnable()
    {
        _data = serializedObject.FindProperty("UpgradeData");
        _upgradesList = new ReorderableList(serializedObject, _data.FindPropertyRelative("UpgradesLevels"), true, true, true, true)
            {
                drawElementCallback = DrawElementCallback,
                elementHeightCallback = ElementHeightCallback
            };
    }


    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        _data.FindPropertyRelative("NameLocKey").stringValue = EditorExtensions.GetLocalizationTermFieldLayout(_data.FindPropertyRelative("NameLocKey").stringValue, "Name Localization Key");
        _data.FindPropertyRelative("DescriptionLocKey").stringValue = EditorExtensions.GetLocalizationTermFieldLayout(_data.FindPropertyRelative("DescriptionLocKey").stringValue, "Description Localization Key");
        EditorGUILayout.PropertyField(_data.FindPropertyRelative("Icon"));
        
        _upgradesList.DoLayoutList();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private float ElementHeightCallback(int index)
    {
        var arraySize = _upgradesList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("Descriptions")
            .arraySize;
        
        return EditorGUIUtility.singleLineHeight * 3 +
               arraySize * EditorGUIUtility.singleLineHeight * 2 + 11 *arraySize;
    }
    
     private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _upgradesList.serializedProperty.GetArrayElementAtIndex(index);

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
            
            var arguments = new object[description.FindPropertyRelative("Values").arraySize];
            var descriptionText = LocalizationManager.GetTranslation(description.FindPropertyRelative("DescriptionKey").stringValue);
            
            for(var i = 0 ; i < arguments.Length ; i ++)
                descriptionText = descriptionText.Replace("{" + i + "}", description.FindPropertyRelative("Values").GetArrayElementAtIndex(i).floatValue.ToString());
            
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

}
