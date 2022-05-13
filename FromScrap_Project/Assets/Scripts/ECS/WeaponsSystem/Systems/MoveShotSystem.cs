using DamageSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using WeaponsSystem.Base.Components;

namespace WeaponsSystem.Base.Systems
{
    public partial class MoveShotSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;

        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer();
            var ecbParallel = ecb.AsParallelWriter();
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach((Entity e, ref Translation translation, ref Rotation rotation, ref ShotTemporaryData tempData, in ShotData shotData, in LocalToWorld localToWorld) =>
            {
                if (tempData.MoveShot)
                {
                    var t = tempData.CurrentLife / shotData.Lifetime;
                    var velocity = tempData.MoveDir * shotData.Velocity * tempData.SpeedMultiplier;
                    
                    var x = velocity.x * t;
                    var z = velocity.z * t;
                    var y = velocity.y * t - shotData.Gravity * math.pow(t, 2) / 2;

                    var prevPos = translation.Value;
                    translation.Value = tempData.InitialPosition + new float3(x, y, z);
                    
                    var dir = math.normalize(translation.Value - prevPos);

                    rotation.Value = quaternion.LookRotation(dir, math.up());
                }

                tempData.CurrentLife += deltaTime;

                if (tempData.CurrentLife >= shotData.Lifetime)
                    ecbParallel.AddComponent<Dead>(e.Index, e);
                
            }).ScheduleParallel();
            
            _ecbSystem.AddJobHandleForProducer(Dependency);
        }
    }
}