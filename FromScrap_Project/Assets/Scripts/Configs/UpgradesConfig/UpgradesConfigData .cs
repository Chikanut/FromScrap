using System;
using System.Collections.Generic;
using UnityEngine;
using Visartech.Progress;

namespace Packages.Common.Storage.Config.Upgrades
{
    [Serializable]
    public class UpgradesConfigData 
    {
        public List<KitConfigScriptable> Kits = new List<KitConfigScriptable>();

        public List<KitConfigScriptable> GetAllOpenedKits()
        {
            var result = new List<KitConfigScriptable>();
            foreach (var kit in Kits)
            {
                if (kit.Data.Opened || Progress.Backpack.GetItem(kit.Data.ID).Count > 0)
                {
                    result.Add(kit);
                }
            }

            return result;
        }

        public (int index, KitConfigScriptable data) GetKit(string kitID)
        {
            for(int i = 0 ; i < Kits.Count ; i ++)
            {
                if (Kits[i].Data.ID == kitID)
                {
                    return (i, Kits[i]);
                }
            }

            Debug.LogError("Kit with id " + kitID + " not found");
            
            return (-1, null);
        }

        public int GetKitIndex(string kitID)
        {
            for (int i = 0; i < Kits.Count; i++)
            {
                if (Kits[i].Data.ID == kitID)
                {
                    return i;
                }
            }

            Debug.LogError("Kit with id " + kitID + " not found");

            return -1;
        }
    }
}
