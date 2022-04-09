using IsVisible.Components;
using Unity.Entities;
using UnityEngine;

namespace IsVisible.Authoring
{
    public class IsVisibleAuthoring : MonoBehaviour , IConvertGameObjectToEntity
    {
        public float ObjectRadius;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var isVisibleData = new IsVisibleComponent()
            {
                ObjectRadius = ObjectRadius
            };
            
            dstManager.AddComponentData(entity, isVisibleData);
        }

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, ObjectRadius);
        }
    }
}