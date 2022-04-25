using DamageSystem.Components;
using Unity.Entities;
using UnityEngine;

namespace DamageSystem.Systems
{
    public partial class CleanUpHealthUISystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            Entities.WithNone<AddHealthBarData>().ForEach((Entity e, in HealthBarUI healthUI) =>
            {
                if(healthUI.SliderContainer != null)
                    Object.Destroy(healthUI.SliderContainer.gameObject);
                ecb.RemoveComponent<HealthBarUI>(e);
            }).WithoutBurst().Run();
        }
    }
}