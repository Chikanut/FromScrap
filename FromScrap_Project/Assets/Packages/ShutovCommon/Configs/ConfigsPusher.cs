using Configs.GameResourcesManagerConfig.BaseResources;
using Configs.GameResourcesManagerConfig.Controllers;
using Packages.Common.Storage.Config;
using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.EnemySpawner;
using Packages.Common.Storage.Config.Upgrades;
using ShootCommon.Signals;
using Signals;
using UniRx;
using UnityEngine;
using Zenject;

public class ConfigsPusher : MonoBehaviour
{
    [SerializeField] private EnemySpawnerConfigScriptable _enemySpawnerConfig;
    [SerializeField] private CarsConfigScriptable _carsConfig;
    [SerializeField] private SoundsConfig _soundsConfig;
    [SerializeField] private PlayerProgressionConfigScriptable _playerProgressionConfig;
    [SerializeField] private UpgradesConfigScriptable _upgradesConfig;
    [SerializeField] private BaseResourcesManagerConfig _baseResourcesManagerConfig;
    
    private ISignalService _signalService;
    private IEnemySpawnerConfigController _enemySpawnerConfigController;
    private ICarsConfigController _carsConfigController;
    private ISoundConfigController _soundConfigController;
    private IPlayerProgressionConfigController _playerProgressionConfigController;
    private IUpgradesConfigController _upgradesConfigController;
    private IGameResourcesManagerConfigController _gameResourcesManagerConfigController;

    private CompositeDisposable _disposeOnExit = new CompositeDisposable();

    [Inject]
    public void Inject(
        ISignalService signalService,
        IEnemySpawnerConfigController enemySpawnerConfigController,
        ICarsConfigController carsConfigController,
        ISoundConfigController soundConfigController,
        IPlayerProgressionConfigController playerProgressionConfig,
        IUpgradesConfigController upgradesConfigController,
        IGameResourcesManagerConfigController gameResourcesManagerConfigController
    )
    {
        _signalService = signalService;
        _enemySpawnerConfigController = enemySpawnerConfigController;
        _carsConfigController = carsConfigController;
        _soundConfigController = soundConfigController;
        _playerProgressionConfigController = playerProgressionConfig;
        _upgradesConfigController = upgradesConfigController;
        _gameResourcesManagerConfigController = gameResourcesManagerConfigController;

        signalService.Receive<LoadGameInfoSignal>().Subscribe(ParsConfig).AddTo(_disposeOnExit);
    }

    private void OnDestroy()
    {
       _disposeOnExit.Dispose();
    }

    private void ParsConfig(LoadGameInfoSignal signal)
    {
        ParsEnemySpawnerConfig();
        ParsUpgradesConfig();
        ParseSoundsConfig();
        ParsePlayerProgressionConfig();
        ParseUpgradesConfig();
        ParseGameResourcesConfig();

        _signalService.Publish(new GameInfoUpdatedSignal());
    }
    
    private void ParsEnemySpawnerConfig()
    {
        if (_enemySpawnerConfig == null) return;
        _enemySpawnerConfigController.SetInfo(_enemySpawnerConfig);
    }
    
    private void ParsUpgradesConfig()
    {
        if (_carsConfigController == null) return;
        _carsConfigController.SetInfo(_carsConfig);
    }

    private void ParseSoundsConfig()
    {
        if(_soundConfigController == null) return;
        _soundConfigController.SetInfo(_soundsConfig);
    }
    
    private void ParsePlayerProgressionConfig()
    {
        if(_playerProgressionConfigController == null) return;
        _playerProgressionConfigController.SetInfo(_playerProgressionConfig);
    }

    private void ParseUpgradesConfig()
    {
        if(_upgradesConfigController == null) return;
        _upgradesConfigController.SetInfo(_upgradesConfig);
    }
    
    private void ParseGameResourcesConfig()
    {
        if(_gameResourcesManagerConfigController == null)
            return;
        
        if(_baseResourcesManagerConfig == null)
            return;
        
        _gameResourcesManagerConfigController.SetInfo(_baseResourcesManagerConfig);
    }
}
