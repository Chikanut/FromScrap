using Cars.View.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

namespace Cars.View.Authorings
{
    public class CarBodyAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Collision Check")]
        public float CarHeigh;
        public float2 CarSize;
        
        private float3[] BodyPoints;
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

            var parent = dstManager.GetComponentData<Parent>(entity).Value;
            
            var bodyInfo = new CarBodyData()
            {
                Parent = parent,
                Anchor = transform.localPosition,
                RotationDamping = RotationDamping,
                MovementDamping = SuspensionDamping,
                CurrentForward = math.forward(),
                CurrentUp = math.up()
                // LocalRotation = transform.localEulerAngles
            };

            dstManager.AddComponentData(entity, bodyInfo);
            var buffer = dstManager.AddBuffer<MultyGroundInfoData>(parent);

            BodyPoints = new[]
            {
                new float3(CarSize.x, 0, CarSize.y),
                new float3(-CarSize.x, 0, CarSize.y),
                new float3(-CarSize.x, 0, -CarSize.y),
                new float3(CarSize.x, 0, -CarSize.y)
            };
            
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
                DrawCross(transform.TransformPoint(new Vector3(CarSize.x,0,CarSize.y)));
                DrawCross(transform.TransformPoint(new Vector3(-CarSize.x,0,CarSize.y)));
                DrawCross(transform.TransformPoint(new Vector3(CarSize.x,0,-CarSize.y)));
                DrawCross(transform.TransformPoint(new Vector3(-CarSize.x,0,-CarSize.y)));
                
                Debug.LogWarning("Wheel should have parent transform!");
                
                return;
            }
            
            DrawLineDown(transform.TransformPoint(new Vector3(CarSize.x,0,CarSize.y)));
            DrawLineDown(transform.TransformPoint(new Vector3(-CarSize.x,0,CarSize.y)));
            DrawLineDown(transform.TransformPoint(new Vector3(CarSize.x,0,-CarSize.y)));
            DrawLineDown(transform.TransformPoint(new Vector3(-CarSize.x,0,-CarSize.y)));
        }

        void DrawCross(Vector3 startPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint - transform.forward * CarHeigh/2 + Vector3.down * CarHeigh/2,
                startPoint + transform.forward * CarHeigh/2 + Vector3.up * CarHeigh/2);

            Gizmos.DrawLine(startPoint + transform.forward * CarHeigh/2 + Vector3.down * CarHeigh/2,
                startPoint - transform.forward * CarHeigh/2 + Vector3.up * CarHeigh/2);
        }

        void DrawLineDown(Vector3 startPoint)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint, startPoint + Vector3.down * CarHeigh);
        }
#endif
    }
}