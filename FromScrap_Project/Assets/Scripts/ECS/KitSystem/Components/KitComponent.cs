using System;
using Unity.Entities;

namespace Kits.Components
{
    public struct KitComponent : IComponentData
    {
        public int ID;
        public KitType Type;
        public bool IsStacked;
        public int KitLevel;
    }
}