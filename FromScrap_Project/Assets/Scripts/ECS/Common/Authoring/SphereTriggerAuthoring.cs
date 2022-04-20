using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using Material = Unity.Physics.Material;

namespace ECS.Common.Authoring
{
    public class SphereTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float Radius = 0.5f;
        public PhysicsCategoryTags BelongsTo = PhysicsCategoryTags.Everything;
        public PhysicsCategoryTags CollideWith = PhysicsCategoryTags.Everything;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var sphere = new SphereGeometry
            {
                Center = float3.zero,
                Radius = Radius
            };
            var material = Material.Default;

            material.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
            
            var colliderRef = Unity.Physics.SphereCollider.Create(sphere, new CollisionFilter()
            {
                CollidesWith = CollideWith.Value,
                BelongsTo = BelongsTo.Value
            }, material);

            dstManager.AddComponentData(entity, new PhysicsCollider {Value = colliderRef});
            dstManager.AddSharedComponentData(entity, new PhysicsWorldIndex() {Value = 0});
            dstManager.AddComponentData(entity, new SphereTriggerComponent() {Radius = Radius, PrevRadius = Radius});

            ConvertAncestors(entity, dstManager, conversionSystem);
        }

        public virtual void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}