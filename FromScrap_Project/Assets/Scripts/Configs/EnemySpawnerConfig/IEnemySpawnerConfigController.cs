namespace Packages.Common.Storage.Config.EnemySpawner
{
    public interface IEnemySpawnerConfigController 
    {
        public EnemySpawnerConfigData GetEnemySpawnerData { get; }
        void SetInfo(EnemySpawnerConfigScriptable data);
    }
}