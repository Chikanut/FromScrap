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
        public float SuspensionDamping;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (transform.parent == null)
            {
                Debug.LogError("Wheel has no parent to connect to!");
                return;
            }
            
            var bodyInfo = new CarBodyData()
            {
                Anchor = transform.localPosition,
                RotationDamping = RotationDamping,
                MovementDamping = SuspensionDamping
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
                    },
                });
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (transform.parent == null)
            {
                Gizmos.color = Color.red;

                for (int i = 0; i < BodyPoints.Length; i++)
                {
                    var startPoint = transform.TransformPoint(BodyPoints[i]);
                    
                    Gizmos.DrawLine(startPoint - transform.forward * CarHeigh/2 + Vector3.down * CarHeigh/2,
                        startPoint + transform.forward * CarHeigh/2 + Vector3.up * CarHeigh/2);

                    Gizmos.DrawLine(startPoint + transform.forward * CarHeigh/2 + Vector3.down * CarHeigh/2,
                        startPoint - transform.forward * CarHeigh/2 + Vector3.up * CarHeigh/2);
                }

                Debug.LogWarning("Wheel should have parent transform!");
                
                return;
            }
            
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