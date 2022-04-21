using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public partial class SphereTriggerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        
        base.OnCreate();
    }
    
    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var colliders = GetComponentDataFromEntity<PhysicsCollider>(true);
        
       Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, ref SphereTriggerComponent sphereInfo) =>
        {
            if (sphereInfo.Radius == sphereInfo.PrevRadius || !colliders.HasComponent(entity)) return;
            
            var filter = colliders[entity].Value.Value.Filter;
            var material = Material.Default;
            material.CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents;
                
            ecb.RemoveComponent<PhysicsCollider>(entityInQueryIndex, entity);
            var sphere = new SphereGeometry
            {
                Center = float3.zero,
                Radius = sphereInfo.Radius
            };
            var colliderRef = SphereCollider.Create(sphere, filter, material);

            ecb.AddComponent(entityInQueryIndex, entity, new PhysicsCollider {Value = colliderRef});
            sphereInfo.PrevRadius = sphere.Radius;
        }).WithReadOnly(colliders).ScheduleParallel(Dependency);

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
