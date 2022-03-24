using Unity.Entities;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct ShotPrefab : IComponentData
    {
        public Entity Value;
    }
}
