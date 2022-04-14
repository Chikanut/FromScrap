using System.Collections.Generic;
using Kits.Authoring;
using Kits.Components;
using UnityEngine;

namespace Packages.Common.Storage.Config.Upgrades
{
    [System.Serializable]
    public class KitInfoData
    {
        public KitType Type;
        public bool IsStacked;
        
        public List<KitComponentAuthoring> KitObjects = new List<KitComponentAuthoring>();
    }

    [CreateAssetMenu(fileName = "KitInfo", menuName = "Configs/KitInfo", order = 3)]
    public class KitConfigScriptable : ScriptableObject
    {
        public KitInfoData Data = new KitInfoData();
    }
}