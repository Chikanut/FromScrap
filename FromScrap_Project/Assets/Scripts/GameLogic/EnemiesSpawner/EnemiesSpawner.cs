using System.Linq;
using Packages.Common.Storage.Config;
using ShootCommon.GlobalStateMachine;
using ShootCommon.GlobalStateMachine.States;
using ShootCommon.Signals;
using UniRx;
using Unity.Mathematics;
using Unity.Transforms;
using Zenject;
using Random = UnityEngine.Random;


public class EnemiesSpawner : IInitializable
{
    private readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();
    
    private IEnemySpawnerConfigController _enemySpawnerConfigController;

    public int spawnCount = 200;
    public float spawnRadius = 25;
    
    [Inject]
    public void Init(ISignalService signalService,
        IEnemySpawnerConfigController enemySpawnerConfigController)
    {
        _enemySpawnerConfigController = enemySpawnerConfigController;
        
        signalService.Receive<ChangeStateSignal>().Subscribe((stateSignal) =>
        {
            if(stateSignal.SelectedState == StateMachineTriggers.InitGame)
                SpawnWave();
            if (stateSignal.SelectedState == StateMachineTriggers.EndGame)
            {
                
            }
        }).AddTo(_disposeOnDestroy);
    }
    
    //Need to be for autoinitialize of enemySpawner
    public void Initialize()
    {

    }

    public void SpawnWave()
    {
        var enemiesArray = _enemySpawnerConfigController.GetEnemySpawnerData.SpawnRanges[0].EnemySpawnInfos
            .Select(enemyInfo => enemyInfo.EnemyPrefab).ToList();
        
        for (int i = 0; i < spawnCount; i++)
        {
            EntityPoolManager.Instance.GetObject(enemiesArray[Random.Range(0, enemiesArray.Count)], (entity, manager) =>
            {
                manager.SetComponentData(entity,
                    new Translation
                    {
                        Value = RandomPointOnCircle(new float3(0, Random.Range(5, 25), 0),
                            spawnRadius + Random.Range(-spawnRadius * 0.25f, spawnRadius * 0.25f))
                    });
            });
        }
    }

    float3 RandomPointOnCircle(float3 center, float radius)
    {
        var angle = 2.0f * math.PI * UnityEngine.Random.Range(0f, 360f);

        return new float3(center.x + radius * math.cos(angle), center.y, center.z + radius * math.sin(angle));
    }

 
}
