using ECS.Common.Authoring;
using Magnet.Components;
using Unity.Entities;

namespace Magnet.Authorings
{
    public class MagnetAuthoring : SphereTriggerAuthoring, IConvertGameObjectToEntity
    {
        public float MoveSpeed = 1;
        
        public override void ConvertAncestors(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new MagnetComponent()
            {
                Speed = MoveSpeed
            });
            dstManager.AddBuffer<MagnetTargetsBuffer>(entity);
        }
    }
}