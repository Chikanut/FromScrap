using System;
using System.Collections.Generic;
using Cars.View.Authorings;
using Packages.Common.Storage.Config.Upgrades;
using UnityEngine;

namespace Packages.Common.Storage.Config.Cars
{
    [CreateAssetMenu(fileName = "CarsConfig", menuName = "Configs/CarsConfig", order = 1)]
    public class CarsConfigScriptable : ScriptableObject
    {
        [Serializable]
        public class CarInfo
        {
            public CarViewAuthoring Prefab;
            public UpgradesConfigScriptable Upgrades;
            public List<int> DefaultUpgrades = new List<int>();
        }

        public List<CarInfo> CarsData = new List<CarInfo>();
    }
}