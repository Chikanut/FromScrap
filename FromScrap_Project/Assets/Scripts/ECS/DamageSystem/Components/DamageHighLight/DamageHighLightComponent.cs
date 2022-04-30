using Unity.Entities;

namespace DamageSystem.Components
{
    [GenerateAuthoringComponent]
    public struct DamageHighLightComponent : IComponentData
    {
        public float HighLightTimeLeft;
        public float HighLightMaxTime;
    }
}
