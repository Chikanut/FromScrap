namespace Packages.Common.Storage.Config.Upgrades
{
    public interface IUpgradesConfigController 
    {
        UpgradesConfigData GetUpgradesData { get; }
        void SetInfo(UpgradesConfigScriptable data);
    }
}