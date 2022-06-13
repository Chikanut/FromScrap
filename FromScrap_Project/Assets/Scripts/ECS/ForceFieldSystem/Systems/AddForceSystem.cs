using ForceField.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace ForceField.Systems
{
    public partial class AddForceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var dt = UnityEngine.Time.fixedDeltaTime;
            
            Dependency = Entities.ForEach((Entity entity, ref PhysicsVelocity bodyVelocity,
                in Translation pos, in PhysicsMass bodyMass, in AddForceComponent addForceComponent) =>
            {
                var strength = addForceComponent.Force;
                var mass = math.rcp(bodyMass.InverseMass);
                
                strength *= mass * dt;
                
                bodyVelocity.Linear += strength * addForceComponent.Dir;
            }).Schedule(Dependency);
        }
    }
    
    [UpdateAfter(typeof(AddForceSystem))]
    public partial class CleanAddForceSystem : SystemBase
    { 
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBuffer;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBuffer.CreateCommandBuffer();
            
            Dependency = Entities.ForEach((Entity entity, in AddForceComponent addForceComponent) =>
            {
                ecb.RemoveComponent<AddForceComponent>(entity);
            }).Schedule(Dependency);
            
            _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}