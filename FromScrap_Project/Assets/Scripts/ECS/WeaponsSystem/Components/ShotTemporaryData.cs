using Unity.Entities;
using Unity.Mathematics;

namespace WeaponsSystem.Base.Components
{
    public struct ShotTemporaryData : IComponentData
    {
        public bool MoveShot;
        public float CurrentLife;
        public float3 MoveDir;
        public float3 InitialPosition;
    }
}