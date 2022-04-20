using MoveTo.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace MoveTo.Systems
{
    public partial class MoveToSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var positions = GetComponentDataFromEntity<LocalToWorld>(true);
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((Entity entity, ref Translation translate, in LocalToWorld localToWorld,
                in MoveToComponent moveToComponent) =>
            {
                if (!positions.HasComponent(moveToComponent.Target)) return;
                
                var targetPosition = positions[moveToComponent.Target].Position;
                if(math.distance(targetPosition, localToWorld.Position) < 1f) return;


                var dir = math.normalize(targetPosition - localToWorld.Position);
                var moveVector = dir * deltaTime * moveToComponent.Speed;
                translate.Value += moveVector;
            }).WithReadOnly(positions).ScheduleParallel();
        }
    }
}