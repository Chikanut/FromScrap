namespace Packages.Common.Storage.Config.Cars
{
    public interface ICarsConfigController 
    {
        CarsConfigData GetUpgradesData { get; }

        CarConfigData GetCarData(int ID);
        void SetInfo(CarsConfigScriptable data);
    }
}