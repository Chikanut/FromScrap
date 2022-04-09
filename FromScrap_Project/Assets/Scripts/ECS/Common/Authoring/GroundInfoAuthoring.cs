using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class GroundInfoAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float CheckDistance;
    public PhysicsCategoryTags BelongsTo;
    public PhysicsCategoryTags CollideWith;


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var checkGround = new GroundInfoData()
        {
            CheckDistance = CheckDistance,
            CollisionFilter = new CollisionFilter()
            {
                CollidesWith = CollideWith.Value,
                BelongsTo = BelongsTo.Value
            }
        };

        dstManager.AddComponentData(entity, checkGround);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position - Vector3.up * CheckDistance);
    }
#endif
}
