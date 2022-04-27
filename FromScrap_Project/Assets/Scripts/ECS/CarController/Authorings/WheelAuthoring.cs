using System.Collections.Generic;
using Cars.View.Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;
using VertexFragment;

namespace Cars.View.Authorings
{
    public class WheelAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class Wheel
        {
            public float WheelSkinWidth;
            public float Radius;
            public float SuspensionDistance;
            public float SuspensionDamping;
            public float SuspensionStrength;
            
            public bool isGuide;
            // public bool isLeft;
            [Range(0, 1)]
            public float TurnRange;
            public float TurnDamping;

            public PhysicsCategoryTags BelongsTo;
            public PhysicsCategoryTags CollideWith;
        }

        [Header("Car Parts")] [SerializeField] private Wheel wheel;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (transform.parent == null)
            {
                Debug.LogError("Wheel has no parent to connect to!");
                return;
            }
            
            var parent = dstManager.GetComponentData<Parent>(entity).Value;

            var wheelData = new WheelData()
            {
                Parent = parent,
                Radius = wheel.Radius,
                SuspensionDistance = wheel.SuspensionDistance,
                SuspensionDamping = wheel.SuspensionDamping,
                isGuide = wheel.isGuide,
                TurnRange = wheel.TurnRange,
                TurnDamping = wheel.TurnDamping,
                SuspensionStrength = wheel.SuspensionStrength
            };

            var checkGround = new GroundInfoData()
            {
                CheckDistance = wheel.Radius + wheel.WheelSkinWidth,
                CollisionFilter = new CollisionFilter()
                {
                    CollidesWith = wheel.CollideWith.Value,
                    BelongsTo = wheel.BelongsTo.Value
                }
            };
            
            dstManager.AddComponentData(entity, wheelData);
            dstManager.AddComponentData(entity, checkGround);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            if (transform.parent == null)
            {
                Gizmos.color = Color.red;
                
                Gizmos.DrawLine(transform.position - transform.forward * wheel.Radius + Vector3.down * wheel.Radius,
                    transform.position + transform.forward * wheel.Radius + Vector3.up * wheel.Radius);
                
                Gizmos.DrawLine(transform.position + transform.forward * wheel.Radius + Vector3.down * wheel.Radius,
                    transform.position - transform.forward * wheel.Radius + Vector3.up * wheel.Radius);
                
                Debug.LogWarning("Wheel should have parent transform!");
                
                return;
            }
            
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.right, wheel.Radius);
            var startPoint = transform.parent.position;
            Gizmos.DrawLine(startPoint, startPoint - transform.parent.up * wheel.SuspensionDistance);

        }
#endif
    }
}