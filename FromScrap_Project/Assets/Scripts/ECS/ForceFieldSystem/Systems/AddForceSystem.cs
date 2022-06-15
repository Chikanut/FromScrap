using ForceField.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using VertexFragment;

namespace ForceField.Systems
{
    
    [UpdateBefore(typeof(AddForceSystem))]
    public partial class AddRandomForceSystem : SystemBase
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
            var randomIndex = GetSingleton<RandomIndex>().Value;
            
            SetSingleton(new RandomIndex() {Value = randomIndex + 1});
            
            Dependency = Entities.ForEach((Entity entity, int entityInQueryIndex, in AddRandomForceComponent addForceComponent) =>
            {
                ecb.AddComponent<AddForceComponent>(entity, new AddForceComponent()
                {
                    Dir = MathUtils.AddSprayToDir(addForceComponent.Direction, addForceComponent.Spray, randomIndex + entityInQueryIndex),
                    Force = addForceComponent.Force
                });
                
                ecb.RemoveComponent<AddRandomForceComponent>(entity);
            }).Schedule(Dependency);
            
            _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
    
    public partial class AddForceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Dependency = Entities.ForEach((Entity entity, ref PhysicsVelocity bodyVelocity,
                in Translation pos, in PhysicsMass bodyMass, in AddForceComponent addForceComponent) =>
            {
                var strength = addForceComponent.Force;
                var mass = math.rcp(bodyMass.InverseMass);
                
                strength *= mass;
                
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