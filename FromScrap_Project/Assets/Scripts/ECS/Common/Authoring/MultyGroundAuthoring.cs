using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class MultyGroundAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float2 BodySize;
    public float CheckHeigh;
    private float3[] BodyPoints;
    public PhysicsCategoryTags BelongsTo;
    public PhysicsCategoryTags CollideWith;
    
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buffer = dstManager.AddBuffer<MultyGroundInfoData>(entity);

        BodyPoints = new[]
        {
            new float3(BodySize.x, 0, BodySize.y),
            new float3(-BodySize.x, 0, BodySize.y),
            new float3(-BodySize.x, 0, -BodySize.y),
            new float3(BodySize.x, 0, -BodySize.y)
        };
            
        for (int i = 0; i < BodyPoints.Length; i++)
        {
            buffer.Add(new MultyGroundInfoData()
            {
                AnchorPoints = BodyPoints[i],
                CheckDistance = CheckHeigh,
                CollisionFilter = new CollisionFilter()
                {
                    CollidesWith = CollideWith.Value,
                    BelongsTo = BelongsTo.Value
                },
            });
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawLineDown(transform.TransformPoint(new Vector3(BodySize.x,0,BodySize.y)));
        DrawLineDown(transform.TransformPoint(new Vector3(-BodySize.x,0,BodySize.y)));
        DrawLineDown(transform.TransformPoint(new Vector3(BodySize.x,0,-BodySize.y)));
        DrawLineDown(transform.TransformPoint(new Vector3(-BodySize.x,0,-BodySize.y)));
    }
    
    void DrawLineDown(Vector3 startPoint)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPoint, startPoint + Vector3.down * CheckHeigh);
    }
#endif
}
