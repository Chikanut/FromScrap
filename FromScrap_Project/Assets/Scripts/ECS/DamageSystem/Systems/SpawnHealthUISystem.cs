using DamageSystem.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace DamageSystem.Systems
{
    public partial class SpawnHealthUISystem : SystemBase
    {
        private GameObject _healthBarPrefab;
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            _healthBarPrefab = Resources.Load("HealthBar") as GameObject;
   
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            Entities.WithAll<AddHealthBarData>().WithNone<HealthBarUI>().ForEach((Entity e, in AddHealthBarData healthBarInfo) =>
            {
                var spawnedHealthBar = Object.Instantiate(_healthBarPrefab);
                
                var healthBarData = new HealthBarUI()
                {
                    Offset = healthBarInfo.Offset,
                    SliderContainer = spawnedHealthBar.transform,
                    Slider = spawnedHealthBar.GetComponentInChildren<Slider>()
                };
                
                ecb.AddComponent<HealthBarUI>(e);
                ecb.SetComponent(e, healthBarData);
            }).WithoutBurst().Run();
        }
    }
}