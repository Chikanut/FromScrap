using StatisticsSystem.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace WeaponsSystem.Base.Components
{
    public struct ShotTemporaryData : IComponentData
    {
        public bool MoveShot;
        
        public float3 Direction;

        public float3 CurrentDirection;
        public float3 DirVelocity;
        
        public float CurrentSpeed;
        public float SpeedVelocity;
        
        public float3 InitialPosition;
        
        public Characteristics Characteristics;
    }
}