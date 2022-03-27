using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace Cars.View.Authorings
{
    public class CarBodyAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Collision Check")]
        public float CarHeigh;
        public float3[] BodyPoints;
        public PhysicsCategoryTags BelongsTo;
        public PhysicsCategoryTags CollideWith;

        [Header("Body Settings")]
        public float RotationDamping;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var bodyInfo = new CarBodyData()
            {
                
            };

            dstManager.AddComponentData(entity, bodyInfo);
            var buffer = dstManager.AddBuffer<MultyGroundInfoData>(entity);

            for (int i = 0; i < BodyPoints.Length; i++)
            {
                buffer.Add(new MultyGroundInfoData()
                {
                    AnchorPoints = BodyPoints[i],
                    CheckDistance = CarHeigh,
                    CollisionFilter = new CollisionFilter()
                    {
                        CollidesWith = CollideWith.Value,
                        BelongsTo = BelongsTo.Value
                    }
                });
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < BodyPoints.Length; i++)
            {
                var startPoint = transform.TransformPoint(BodyPoints[i]);
                Gizmos.DrawLine(startPoint, startPoint + Vector3.down * CarHeigh);
            }
        }
#endif
    }
}