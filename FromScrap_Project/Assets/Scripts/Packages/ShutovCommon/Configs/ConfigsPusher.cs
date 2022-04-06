using Packages.Common.Storage.Config;
using ShootCommon.Signals;
using UniRx;
using UnityEngine;
using Zenject;

public class ConfigsPusher : MonoBehaviour
{
    [SerializeField] private BaseConfigData _testConfig;

    private ISignalService _signalService;
    private IBaseConfigController _locationsConfigController;

    private CompositeDisposable _disposeOnExit = new CompositeDisposable();
    
    public void Awake()
    {
        ProjectContext.Instance.Container.Inject(this);
    }

    [Inject]
    public void Inject(
        ISignalService signalService,
        IBaseConfigController locationsConfigController
    )
    {
        _signalService = signalService;
        _locationsConfigController = locationsConfigController;

        signalService.Receive<LoadGameInfoSignal>().Subscribe((signal) => { ParsConfig();}) .AddTo(_disposeOnExit);
    }

    private void ParsConfig()
    {
        ParsLocationConfig();

        _signalService.Publish(new ConfigUpdatedSignal());
    }
    
    private void ParsLocationConfig()
    {
        if (_testConfig == null) return;
        _locationsConfigController.SetInfo(_testConfig);
    }

}
