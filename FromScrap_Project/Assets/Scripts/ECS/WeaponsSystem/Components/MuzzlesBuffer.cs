using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace WeaponsSystem.Base.Components
{
    public struct MuzzlesBuffer : IBufferElementData
    {
        public FixedString32Bytes Projectile;

        public float3 Offset;
        public float3 Direction;

        public int ShotsCount;
        public float ShotsAngle;
        public float3 ShotsAngleAxis;
        public float ShootSpray;

        public int ShotAnimationIndex;
    }
}