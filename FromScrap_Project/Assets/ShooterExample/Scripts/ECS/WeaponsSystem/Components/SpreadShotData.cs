using Unity.Entities;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    [WriteGroup(typeof(SpawnShotData))]
    public struct SpreadShotData : IComponentData
    {
        public int NumberOfShots;
        public float AngleOfShots;
    }
}
