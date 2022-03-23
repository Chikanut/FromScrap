using Unity.Collections;
using Unity.Entities;

namespace WeaponsSystem.Base.Components
{
    [GenerateAuthoringComponent]
    public class WeaponType : IComponentData
    {
        public string  Value;
    }
}
