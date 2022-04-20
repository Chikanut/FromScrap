using Packages.Common.Storage.Config.Cars;
using UnityEditor;


[CustomEditor(typeof(CarsConfigScriptable))]
public class CarsConfigScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var baseInfo = (target as CarsConfigScriptable);
        var carsInfo = baseInfo.CarsData;

        for (int i = 0; i < carsInfo.Count; i++)
        {
            if(carsInfo[i].Prefab == null)
                continue;
            
            var changed = false;

            var IDComponent = carsInfo[i].Prefab.GetComponent<CarIDAuthoring>();
            
            if(IDComponent == null) continue;
            
            if (IDComponent.ID != i)
            {
                IDComponent.ID = i;
                changed = true;
            }

            if (changed)
                PrefabUtility.SavePrefabAsset(carsInfo[i].Prefab.gameObject);
        }
    }
}