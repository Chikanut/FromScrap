using Unity.Entities;
using Unity.Mathematics;

namespace Kits.Components
{
    [GenerateAuthoringComponent]
    public struct KitInstalatorComponent : IComponentData
    {
        public float3 Offset;
        public bool LocalUp;
    }
}
