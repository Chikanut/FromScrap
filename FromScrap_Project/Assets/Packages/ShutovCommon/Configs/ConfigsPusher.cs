using Packages.Common.Storage.Config;
using Packages.Common.Storage.Config.Cars;
using Packages.Common.Storage.Config.EnemySpawner;
using ShootCommon.Signals;
using UniRx;
using UnityEngine;
using Zenject;

public class ConfigsPusher : MonoBehaviour
{
    [SerializeField] private EnemySpawnerConfigScriptable _enemySpawnerConfig;
    [SerializeField] private CarsConfigScriptable _carsConfig;
    
    private ISignalService _signalService;
    private IEnemySpawnerConfigController _enemySpawnerConfigController;
    private ICarsConfigController _carsConfigController;

    private CompositeDisposable _disposeOnExit = new CompositeDisposable();

    [Inject]
    public void Inject(
        ISignalService signalService,
        IEnemySpawnerConfigController enemySpawnerConfigController,
        ICarsConfigController carsConfigController
    )
    {
        _signalService = signalService;
        _enemySpawnerConfigController = enemySpawnerConfigController;
        _carsConfigController = carsConfigController;
        
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

}
