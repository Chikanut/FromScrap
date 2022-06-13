using DamageSystem.Components;
using DamageSystem.Systems;
using ForceField.Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    [UpdateBefore(typeof(DeathCleanupSystem))]
    public partial class ExplosionForceSystem : SystemBase
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

            Dependency = Entities.WithAll<Dead>().ForEach((Entity entity, in LocalToWorld localToWorld, in ShotData shotData, in ShotTemporaryData tempShotData, in DealDamage dealDamage, in PhysicsCollider collider) =>
            {
                var createForceFieldEntity = ecb.CreateEntity();

                var hitForceDamage = shotData.HitForceDamage;
                hitForceDamage.Value = (int)(hitForceDamage.Value * tempShotData.Characteristics.DamageMultiplier);
                hitForceDamage.Value += tempShotData.Characteristics.AdditionalDamage;
                
                if(shotData.HitForce.Radius <= 0) return;
                
                ecb.AddComponent(createForceFieldEntity, new CreateForceFieldComponent()
                {
                    CollisionFilter = collider.Value.Value.Filter,
                    DealDamageInfo = hitForceDamage,
                    ForceFieldInfo = shotData.HitForce,
                    Position = localToWorld.Position
                });
            }).Schedule(Dependency);
            
            _endSimulationEntityCommandBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}