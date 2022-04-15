using System;
using System.Collections.Generic;
using Packages.Common.Storage.Config.Upgrades;
using UnityEngine;

namespace Packages.Common.Storage.Config.Cars
{
    public class CarConfigData
    {
        public int ID;
        public GameObject Prefab;
        public List<KitInfoData> UpgradesConfigs = new List<KitInfoData>();
    }

    [Serializable]
    public class CarsConfigData
    {
        public List<CarConfigData> CarsConfigs = new List<CarConfigData>();
    }
}
