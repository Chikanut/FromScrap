namespace Packages.Common.Storage.Config
{
    public interface IEnemySpawnerConfigController 
    {
        public EnemySpawnerConfigData GetEnemySpawnerData { get; }
        void SetInfo(EnemySpawnerConfigScriptable data);
    }
}