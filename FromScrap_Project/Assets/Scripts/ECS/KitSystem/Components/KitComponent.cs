using System;
using Unity.Entities;

namespace Kits.Components
{
    public struct KitComponent : IComponentData
    {
        public Entity Platform;
        public KitType Type;
        public bool IsStacked;
        public int KitLevel;
    }
}