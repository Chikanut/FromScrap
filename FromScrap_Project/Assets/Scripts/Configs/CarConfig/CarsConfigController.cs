using System.Linq;

namespace Packages.Common.Storage.Config.Cars
{
    public class CarsConfigController : ICarsConfigController
    {
        private CarsConfigData _data;
        public CarsConfigData GetUpgradesData => _data;

        public CarConfigData GetCarData(int ID)
        {
            return _data.CarsConfigs[ID];
        }

        public void SetInfo(CarsConfigScriptable data)
        {
            _data = new CarsConfigData();
            for (int i = 0; i < data.CarsData.Count; i++)
            {
                _data.CarsConfigs.Add(new CarConfigData()
                {
                    ID = i,
                    Prefab = data.CarsData[i].Prefab.gameObject,
                    BaseCharacteristics = data.CarsData[i].BaseSettings.BaseStats,
                    LevelsExperience = data.CarsData[i].Levels.LevelsExperience,
                    UpgradesConfigs = data.CarsData[i].Upgrades.Kits.Select(kitScriptable=>kitScriptable.Data).ToList(),
                });
            }
        }
    }
}