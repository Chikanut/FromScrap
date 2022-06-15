using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config.Upgrades
{
    [CreateAssetMenu(fileName = "UpgradesConfig", menuName = "Configs/UpgradesConfig", order = 1)]
    public class UpgradesConfigScriptable : ScriptableObject
    {
        [SerializeField] public List<KitConfigScriptable> Kits = new List<KitConfigScriptable>();

        public KitConfigScriptable GetKit(string kitId)
        {
            foreach (var kit in Kits)
            {
                if (kit.Data.ID == kitId)
                {
                    return kit;
                }
            }

            Debug.LogError("Kit with id " + kitId + " not found");
            
            return null;
        }
    }
}
