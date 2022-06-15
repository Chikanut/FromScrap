namespace Packages.Common.Storage.Config.Upgrades
{
    public class UpgradesConfigController : IUpgradesConfigController
    {
        private UpgradesConfigData _model;
        public UpgradesConfigData GetUpgradesData => _model;
        public void SetInfo(UpgradesConfigScriptable data)
        {
            _model = new UpgradesConfigData
            {
                Kits = data.Kits
            };
        }
    }
}