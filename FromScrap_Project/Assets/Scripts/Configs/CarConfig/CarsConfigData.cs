using System;
using System.Collections.Generic;
using Packages.Common.Storage.Config.Upgrades;
using StatisticsSystem.Components;
using UnityEngine;

namespace Packages.Common.Storage.Config.Cars
{
    public class CarConfigData
    {
        public int ID;
        public GameObject Prefab;
        public GameObject PresentationPrefab;
        public Characteristics BaseCharacteristics;
        public List<int> LevelsExperience = new List<int>();
    }

    [Serializable]
    public class CarsConfigData
    {
        public List<CarConfigData> CarsConfigs = new List<CarConfigData>();
    }
}
