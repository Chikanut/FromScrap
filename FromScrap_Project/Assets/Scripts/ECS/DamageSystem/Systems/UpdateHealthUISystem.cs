using DamageSystem.Components;
using Unity.Entities;
using Unity.Transforms;

namespace DamageSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class UpdateHealthUISystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((HealthBarUI healthUI, in Health health, in Translation translation) =>
            {
                healthUI.SliderContainer.position = translation.Value + healthUI.Offset;
                healthUI.Slider.value = (float)health.Value / health.CurrentMaxValue;
            }).WithoutBurst().Run();
        }
    }
}