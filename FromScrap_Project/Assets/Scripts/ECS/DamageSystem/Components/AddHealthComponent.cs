using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DamageSystem.Components
{
    public struct AddHealthComponent : IComponentData
    {
        public int Value;
    }
}
