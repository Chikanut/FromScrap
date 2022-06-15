using System;
using System.Collections.Generic;
using Kits.Authoring;
using Kits.Components;
using UnityEngine;
using Visartech.Progress;

namespace Packages.Common.Storage.Config.Upgrades
{
    [Serializable]
    public class KitComponentData
    {
        public KitComponentAuthoring Authoring;

        [Serializable]
        public class Description
        {
            public string DescriptionKey;
            public float[] Values;
        }

        public List<Description> Descriptions = new List<Description>();
    }

    [Serializable]
    public class KitInfoData
    {
        public string ID;
        
        [Header("Info")]
        public string NameLocKey;
        public string DescriptionLocKey;
        public Sprite Icon;
        
        [Header("Settings")]
        public KitType Type;
        public bool IsStacked;
        public bool isDefault;
        public bool Opened;
        
        public List<KitComponentData> KitObjects = new List<KitComponentData>();
    }

    [CreateAssetMenu(fileName = "KitInfo", menuName = "Configs/Cars/KitInfo", order = 3)]
    public class KitConfigScriptable : ScriptableObject
    {
        public KitInfoData Data = new KitInfoData();

        public bool IsOpened => Data.Opened || Progress.Backpack.GetItem(Data.ID).Count > 0;
    }
}