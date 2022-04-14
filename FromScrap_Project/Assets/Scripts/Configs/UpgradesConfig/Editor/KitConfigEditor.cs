using Packages.Common.Storage.Config.Upgrades;
using UnityEditor;


[CustomEditor(typeof(KitConfigScriptable))]
public class KitConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var baseInfo = (target as KitConfigScriptable);
        var kitsInfo = baseInfo.Data.KitObjects;

        for (int i = 0; i < kitsInfo.Count; i++)
        {
            var changed = false;
            
            if (kitsInfo[i].Type != baseInfo.Data.Type)
            {
                kitsInfo[i].Type = baseInfo.Data.Type;
                changed = true;
            }

            if (kitsInfo[i].IsStacked != baseInfo.Data.IsStacked)
            {
                kitsInfo[i].IsStacked = baseInfo.Data.IsStacked;
                changed = true;
            }
            
            if (kitsInfo[i].KitLevel != i)
            {
                kitsInfo[i].KitLevel = i;
                changed = true;
            }
            
            if (changed)
                PrefabUtility.SavePrefabAsset(kitsInfo[i].gameObject);
        }
    }
}
