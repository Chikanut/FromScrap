using System;
using System.Collections.Generic;
using UnityEngine;
using Vehicles.Wheels.Authorings;

namespace Packages.Common.Storage.Config.Cars
{
    [CreateAssetMenu(fileName = "CarsConfig", menuName = "Configs/Cars/CarsConfig", order = 1)]
    public class CarsConfigScriptable : ScriptableObject
    {
        [Serializable]
        public class CarInfo
        {
            public VehicleAuthoring Prefab;
            public VehicleAuthoring PresentationPrefab;
            public CarBaseCharacterisitcsScriptable BaseSettings;
            public LevelsConfigScriptable Levels;
        }

        public List<CarInfo> CarsData = new List<CarInfo>();
    }
}