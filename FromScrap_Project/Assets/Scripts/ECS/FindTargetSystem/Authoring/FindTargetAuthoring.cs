using ECS.Common;
using Unity.Entities;
using UnityEngine;

namespace ECS.FindTargetSystem
{
    public class FindTargetAuthoring : QuadrantObjectAuthoring
    {
        [Header("Find Target Settings")] [SerializeField]
        private EntityObjectType TargetType;

        [SerializeField] private float Range;

        protected override void ConvertAncestors(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            base.ConvertAncestors(entity, dstManager, conversionSystem);

            var findClosestTarget = new FindTargetData()
            {
                TargetType = TargetType
            };

            dstManager.AddSharedComponentData(entity, findClosestTarget);
            dstManager.AddComponentData(entity, new TargetSearchRadiusComponent() {Radius = Range});
        }
    }
}
