using System;
using Unity.Entities;

namespace Kits.Components
{
    [GenerateAuthoringComponent]
    public struct KitComponent : IComponentData
    {
        public UInt16 ID;
        public KitType Type;
        public bool IsStacked;
        public int KitLevel;
    }
}