using Unity.Entities;
using UnityEngine;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public struct ShotPrefab : IComponentData
    {
        public Entity Value;
    }
}
