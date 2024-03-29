using System.Collections.Generic;
using Packages.Common.Storage.Config.EnemySpawner;
using Packages.Utils.Extensions;
using ShootCommon.GlobalStateMachine;
using ShootCommon.GlobalStateMachine.States;
using ShootCommon.Signals;
using UniRx;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using VertexFragment;
using Zenject;
using Plane = UnityEngine.Plane;
using Random = UnityEngine.Random;

public partial class EnemiesSpawnerSystem : SystemBase
{
    public class EnemiesSpawnerData
    {
        public float CurrentRunTime;
        public float PrevSpawnedEnemyTime;
        public int NextSpawnRange;
        public float NextSpawnRangeTime;
        public int NextSpecialEnemy = 0;
        public EnemySpawnerConfigData.SpawnRange CurrentSpawnRange;
    }
    
  private readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();
    
    private IEnemySpawnerConfigController _enemySpawnerConfigController;
    private BuildPhysicsWorld _physicsWorldSystem;
    
    private EnemiesSpawnerData _data;
    private bool _isSpawning;
    private Plane _groundPlane;
    private Vector2 _screenSize;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        RequireSingletonForUpdate<PlayerMovementInputComponent>();
        
        _physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
        _screenSize = new Vector2(Screen.width, Screen.height);
        
        ProjectContext.Instance.Container.Inject(this);
    }

    protected override void OnDestroy()
    {
        _disposeOnDestroy.Dispose();
        base.OnDestroy();
    }

    [Inject]
    public void Init(ISignalService signalService,
        IEnemySpawnerConfigController enemySpawnerConfigController)
    {
        _enemySpawnerConfigController = enemySpawnerConfigController;

        signalService.Receive<ChangeStateSignal>().Subscribe((stateSignal) =>
        {
            if(stateSignal.SelectedState == StateMachineTriggers.Game)
                StartSpawning();
            if (stateSignal.SelectedState == StateMachineTriggers.EndGame)
                EndSpawning();
        }).AddTo(_disposeOnDestroy);
    }

    void StartSpawning()
    {
        _screenSize = new Vector2(Screen.width, Screen.height);
        _data = new EnemiesSpawnerData();
        _isSpawning = true;
    }
    
    protected override void OnUpdate()
    {
        if(!_isSpawning) return;
        
        UpdateRangeTime();
        SpawnSpecialEnemy();
        SpawnEnemy();
    }

    void EndSpawning()
    {
        _data = null;
        _isSpawning = false;
    }

    void UpdateRangeTime()
    {
        _data.CurrentRunTime += Time.DeltaTime;

        if (!(_data.NextSpawnRangeTime <= _data.CurrentRunTime)) return;
        
        _data.CurrentSpawnRange =
            _enemySpawnerConfigController.GetEnemySpawnerData.SpawnRanges[_data.NextSpawnRange];
        
        _data.NextSpawnRangeTime = _data.CurrentSpawnRange.RangeSpawnDuration;
        _data.CurrentRunTime = 0;
        _data.NextSpecialEnemy = 0;
        _data.PrevSpawnedEnemyTime = 0;
        
        _data.NextSpawnRange = math.clamp(_data.NextSpawnRange + 1, 0,
            _enemySpawnerConfigController.GetEnemySpawnerData.SpawnRanges.Count - 1);
    }
    
    void SpawnSpecialEnemy()
    {
        if(_data.NextSpecialEnemy >= _data.CurrentSpawnRange.SpecialSpawnInfos.Count) return;
        
        if (_data.CurrentRunTime > _data.CurrentSpawnRange.SpecialSpawnInfos[_data.NextSpecialEnemy].SpawnOnSecond)
        {
            var enemyPrefab = _data.CurrentSpawnRange.SpecialSpawnInfos[_data.NextSpecialEnemy].SpecialEnemyPrefab;

            EntityPoolManager.Instance.GetObject(enemyPrefab , (entity, manager) =>
            {
                manager.SetComponentData(entity,
                    new Translation
                    {
                        Value = GetSpawnPoint(enemyPrefab.GetMaxBounds())
                    });
            });

            _data.NextSpecialEnemy++;
        }
    }

    void SpawnEnemy()
    {
        var spawnDelay =
            _data.CurrentSpawnRange.SpawnIntensityCurve.Evaluate(_data.CurrentRunTime / _data.NextSpawnRangeTime) *
            _data.CurrentSpawnRange.SpawnsPerSecond;
        
        if (!(_data.CurrentRunTime - _data.PrevSpawnedEnemyTime > 1f / spawnDelay)) return;
        
        var enemyPrefab = GetEnemyPrefab();
        var midlPoint = GetCameraMidlePoint();

        EntityPoolManager.Instance.GetObject(enemyPrefab , (entity, manager) =>
        {
            var spawnPoint = GetSpawnPoint(enemyPrefab.GetMaxBounds());
            manager.SetComponentData(entity,
                new Translation
                {
                    Value = spawnPoint
                });
            
            var dir = Vector3.Normalize(midlPoint - spawnPoint);
            dir.y = 0;
            manager.SetComponentData(entity,
                new Rotation
                {
                    Value = Quaternion.LookRotation(dir)
                });
        });

        _data.PrevSpawnedEnemyTime = _data.CurrentRunTime;
    }

    GameObject GetEnemyPrefab()
    {
        var spawnChance = Random.Range(0, 100);
        var enemiesArray = _data.CurrentSpawnRange.EnemySpawnInfos;
        var enemiesToSpawn = new List<GameObject>();
        
        foreach (var enemyInfo in enemiesArray)
        {
            if (enemyInfo.SpawnChance >= spawnChance)
                enemiesToSpawn.Add(enemyInfo.EnemyPrefab);
        }
        
        if(enemiesToSpawn.Count == 0)
            return enemiesArray[enemiesArray.Count - 1].EnemyPrefab;
 
        var index = Random.Range(0, enemiesToSpawn.Count);
        return enemiesToSpawn[index];
    }
    
    float3 GetSpawnPoint(Bounds enemySize)
    {
        var angle = 2.0f * math.PI * Random.Range(0f, 360f);
        var targetDir = new float3(math.cos(angle), math.sin(angle),0);
        var screenPos = ClampDirInRect(targetDir * float.MaxValue, new float3(_screenSize.x / 2, _screenSize.y / 2, 0));
        var spawnPoint = float3.zero;
        
        screenPos += new float3(_screenSize.x / 2, _screenSize.y / 2, 0);
        
        var ray = Camera.main.ScreenPointToRay(screenPos);
        
        if (_groundPlane.Raycast(ray, out var enter))
            spawnPoint = ray.GetPoint(enter);

        var rect = enemySize.size * _enemySpawnerConfigController.GetEnemySpawnerData.SpawnOffset;
        spawnPoint += ClampDirInRect(new float3(targetDir.x, 0, targetDir.y) * float.MaxValue,rect);
        
        var collisionWorld = _physicsWorldSystem.PhysicsWorld.CollisionWorld;
        var (isHit, hitInfo) = PhysicsUtils.Raycast(spawnPoint + math.up() * 50,
            spawnPoint - math.up() * 50, collisionWorld);

        if (isHit)
            spawnPoint = hitInfo.Position;

        spawnPoint += math.up() * enemySize.size.y;
        
        return spawnPoint;
    }

    float3 GetCameraMidlePoint()
    {
        var midlePoint = float3.zero;
        
        var ray = Camera.main.ScreenPointToRay(new Vector3(_screenSize.x/2, _screenSize.y/2, 0));

        if (_groundPlane.Raycast(ray, out var enter))
            midlePoint = ray.GetPoint(enter);
        
        return midlePoint;
    }

    float3 ClampDirInRect(float3 dir, float3 rectSize)
    {
        if(dir.x != 0 && rectSize.x != 0)
            dir /= math.clamp(Mathf.Abs(dir.x / rectSize.x),1,float.MaxValue);
        if(dir.y != 0 && rectSize.y != 0)
            dir /= math.clamp(Mathf.Abs(dir.y / rectSize.y),1,float.MaxValue);
        if(dir.z != 0 && rectSize.z != 0)
            dir /= math.clamp(Mathf.Abs(dir.z / rectSize.z),1,float.MaxValue);

        return dir;
    }
}
