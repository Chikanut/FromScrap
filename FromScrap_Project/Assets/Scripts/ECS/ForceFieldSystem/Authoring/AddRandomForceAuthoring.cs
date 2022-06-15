using ForceField.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Gizmos = Popcron.Gizmos;
using MathUtils = VertexFragment.MathUtils;

namespace ForceField.Authorings
{
    public class AddRandomForceAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float3 Direction = new float3(0, 1, 0);
        [Range(0,90)]
        public float Spray;
        public float Force;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.Cone(transform.position,
                Quaternion.LookRotation(transform.TransformDirection(Direction)), 0.4f, Spray, Color.green);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new AddRandomForceComponent
            {
                Direction = Direction,
                Force = Force,
                Spray = Spray
            });
        }
    }
}
