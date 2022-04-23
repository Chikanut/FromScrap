using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class GameObjectTrackComponent : IComponentData
{
    public Transform TargetObject;
    public GameObject TargetGameobject;
    public bool Spawn = false;

    public void UpdateEntityPos(Vector3 pos)
    {
        TargetGameobject.transform.position = Vector3.Lerp(TargetGameobject.transform.position, pos, 1f);
    }

    public void SpawnGo()
    {
        if(Spawn)
            return;

        Object.Instantiate(TargetGameobject);

        Spawn = true;
    }
}
