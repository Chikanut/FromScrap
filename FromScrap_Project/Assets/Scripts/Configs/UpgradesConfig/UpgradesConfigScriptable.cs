using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config.Upgrades
{
    [CreateAssetMenu(fileName = "UpgradesConfig", menuName = "Configs/UpgradesConfig", order = 1)]
    public class UpgradesConfigScriptable : ScriptableObject
    {
        [SerializeField] public List<KitConfigScriptable> Kits = new List<KitConfigScriptable>();
    }
}
