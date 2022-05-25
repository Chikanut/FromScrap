using System.Collections.Generic;
using Cars.View.Components;
using ECS.SignalSystems.Systems;
using Kits.Components;
using LevelingSystem.Components;
using Packages.Common.Storage.Config.Cars;
using ShootCommon.Signals;
using StatisticsSystem.Components;
using UniRx;
using Unity.Entities;
using UnityEngine;
using Visartech.Progress;
using Zenject;

public class PlatformInfo
{
    public int ID;
    public List<KitType> Connections = new List<KitType>();
    public bool isFree;
    public bool canOccupy;
    public List<KitComponent> ConnectedKits = new List<KitComponent>();
    public List<KitIDComponent> ConnectedKitsIDs = new List<KitIDComponent>();
}
    
public class CurrentCarInfoData
{
    public CarConfigData carData;
    public Characteristics CurrentCharacteristics;
    public Entity carEntity;
    public int carID;
    public int carLevel;
    public List<PlatformInfo> platformInfos = new List<PlatformInfo>();
}

public class CurrentGameStats
{
    public float Time;
    public int Level;
    public int Damage;
    public int Kills;
    public int CollectedScrap;

    public int ExperienceGained 
    {
        get
        {
            var experience = 0f;

            experience += Time * 10f;
            experience += Level * 100f;
            experience += Damage / 1000f;
            experience += Kills / 10f;
            experience += CollectedScrap / 10f;

            return (int)experience;
        }
    }
}

public class GameData
{
    public GameData()
    {
        Stats = new CurrentGameStats();
    }

    public CurrentCarInfoData CarData;
    public CurrentGameStats Stats;
}

public struct GameTimeChanged : ISignal
{
    public float Time;
}

public struct UpgradesChanged : ISignal
{
    public CurrentCarInfoData CarData;
}

public interface IGameDataController
{
    void Initialize();
    void Update(float deltaTime);
    GameData Data { get; }
}

public class GameDataController : IGameDataController, IInitializable
{
    private ISignalService _signalService;
    private ICarsConfigController _carsConfigController;

    public GameData Data => _data;
    private GameData _data;
    
    private EntityManager _entityManager;
    private readonly CompositeDisposable _disposeOnDestroy = new CompositeDisposable();
    
    public void Initialize()
    {
        _data = new GameData();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    
    [Inject]
    public void Init(ISignalService signalService, ICarsConfigController carsConfigController)
    {
        _signalService = signalService;
        
        _signalService.Receive<KitInstalledSignal>().Subscribe(UpdateCarData).AddTo(_disposeOnDestroy);
        
        _carsConfigController = carsConfigController;
    }

    public void UpdateCarData(KitInstalledSignal signal)
    {
        if (!_entityManager.HasComponent<CarIDComponent>(signal.Car))
        {
            Debug.LogError("There is no car entity, or car ID component on target entity!");
            return;
        }

        var carID = Progress.Player.Car;

        if (_entityManager.GetComponentData<CarIDComponent>(signal.Car).ID != carID)
        {
            Debug.LogError("ID of this car is not equal to current car ID!");
            return;
        }

        _data.CarData = new CurrentCarInfoData();

        InitCarInfo(signal.Car);
        InitKits();

        _signalService.Publish(new UpgradesChanged() {CarData = _data.CarData});
    }
    
    void InitCarInfo(Entity car)
    {
        _data.CarData.carEntity = car;
        _data.CarData.carLevel = _entityManager.GetComponentData<LevelComponent>(car).Level;

        if (car == Entity.Null || !_entityManager.HasComponent<CarIDComponent>(car))
        {
            Debug.LogError("There is no car entity, or car ID component on target entity!");
            return;
        }

        if (_entityManager.HasComponent<CharacteristicsComponent>(car))
        {
            _data.CarData.CurrentCharacteristics = _entityManager.GetComponentData<CharacteristicsComponent>(car).Value;
        }

        _data.CarData.carID = _entityManager.GetComponentData<CarIDComponent>( _data.CarData.carEntity).ID;
        _data.CarData.carData = _carsConfigController.GetCarData( _data.CarData.carID);
    }

    void InitKits()
    {
        var kitsScheme = _entityManager.GetBuffer<KitSchemeBuffer>(_data.CarData.carEntity);
        for (int i = 0; i < kitsScheme.Length; i++)
        {
            var platformComponent = _entityManager.GetComponentData<KitPlatformComponent>(kitsScheme[i].Platform);
            var platformConnections = _entityManager.GetBuffer<KitPlatformConnectionBuffer>(kitsScheme[i].Platform);
            var platformConnectedKits = _entityManager.GetBuffer<KitPlatformKitsBuffer>(kitsScheme[i].Platform);

            var platformInfo = new PlatformInfo()
            {
                ID = i,
                isFree = platformComponent.IsFree,
                canOccupy = platformComponent.CanOccupy,
            };
				
            foreach (var connection in platformConnections)
            {
                platformInfo.Connections.Add(connection.ConnectionType);
            }

            foreach (var kit in platformConnectedKits)
            {
                var kitComponent = _entityManager.GetComponentData<KitComponent>(kit.ConnectedKit);
                var kitID = _entityManager.GetComponentData<KitIDComponent>(kit.ConnectedKit);
                platformInfo.ConnectedKits.Add(kitComponent);
                
                platformInfo.ConnectedKitsIDs.Add(kitID);
            }
				
            _data.CarData.platformInfos.Add(platformInfo);
        }
    }
    
    public void Update(float deltaTime)
    {
        _data.Stats.Time += deltaTime;
        _signalService.Publish(new GameTimeChanged(){Time = _data.Stats.Time});
    }
}
