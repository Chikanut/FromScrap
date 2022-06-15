using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Common.Storage.Config.Upgrades
{
    [Serializable]
    public class UpgradesConfigData 
    {
        public List<KitConfigScriptable> Kits = new List<KitConfigScriptable>();
        
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
