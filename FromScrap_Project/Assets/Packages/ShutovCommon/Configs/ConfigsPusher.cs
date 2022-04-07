using Packages.Common.Storage.Config;
using ShootCommon.Signals;
using UniRx;
using UnityEngine;
using Zenject;

public class ConfigsPusher : MonoBehaviour
{
    [SerializeField] private EnemySpawnerConfigScriptable _enemySpawnerConfig;

    private ISignalService _signalService;
    private IEnemySpawnerConfigController _enemySpawnerConfigController;

    private CompositeDisposable _disposeOnExit = new CompositeDisposable();
    
    public void Awake()
    {
        ProjectContext.Instance.Container.Inject(this);
    }

    [Inject]
    public void Inject(
        ISignalService signalService,
        IEnemySpawnerConfigController enemySpawnerConfigController
    )
    {
        _signalService = signalService;
        _enemySpawnerConfigController = enemySpawnerConfigController;

        signalService.Receive<LoadGameInfoSignal>().Subscribe(ParsConfig).AddTo(_disposeOnExit);
    }

    private void ParsConfig(LoadGameInfoSignal signal)
    {
        ParsEnemySpawnerConfig();
        
        _signalService.Publish(new ConfigUpdatedSignal());
    }
    
    private void ParsEnemySpawnerConfig()
    {
        if (_enemySpawnerConfig == null) return;
        _enemySpawnerConfigController.SetInfo(_enemySpawnerConfig);
    }

}
