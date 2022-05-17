using Unity.Entities;

namespace ECS.DynamicTerrainSystem
{
    public struct DynamicTerrainTrackInfoData : IBufferElementData
    {
        public Entity TrackEntity;
        public bool IsPlayer;
    }
}
