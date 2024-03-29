using DamageSystem.Components;
using Packages.Utils.SoundManager.Signals;
using Unity.Entities;
using UnityEngine;

public class SpawnAfterDeathAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [System.Serializable]
    public class SoundSpawnInfo
    {
        public SoundSpawnInfo()
        {
            Delay = 0;
            Type = SoundType.Default;
            Pitch = 1;
            PitchTime = 0;
        }

        public string ClipName;
        public float Delay = 0;
        public SoundType Type = SoundType.Default;
        public float Pitch = 1;
        public float PitchTime = 0;
    }

    [System.Serializable]
    public class SpawnObjectInfo
    {
        public string ObjectName;
        [Range(0,100)]
        public float SpawnChance;
    }

    [Header("Death Info")]
    [SerializeField] private SpawnObjectInfo[] SpawnEntitiesOnDeath;
    [SerializeField] private SpawnObjectInfo[] SpawnPoolObjectsOnDeath;
    [SerializeField] private SoundSpawnInfo[] SpawnAudioObjectsOnDeath;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (SpawnEntitiesOnDeath != null)
        {
            if (SpawnEntitiesOnDeath.Length > 0)
            {
                var spawnAfterDeathBuffer = dstManager.AddBuffer<SpawnEntityOnDeathBuffer>(entity);
                for (int i = 0; i < SpawnEntitiesOnDeath.Length; i++)
                {
                    spawnAfterDeathBuffer.Add(new SpawnEntityOnDeathBuffer()
                        {SpawnEntity = SpawnEntitiesOnDeath[i].ObjectName, SpawnChance = SpawnEntitiesOnDeath[i].SpawnChance});
                }
            }
        }
            
        if (SpawnPoolObjectsOnDeath != null)
        {
            if (SpawnPoolObjectsOnDeath.Length > 0)
            {
                var spawnAfterDeathBuffer = dstManager.AddBuffer<SpawnPoolObjectOnDeathBuffer>(entity);
                for (int i = 0; i < SpawnPoolObjectsOnDeath.Length; i++)
                {
                    spawnAfterDeathBuffer.Add(new SpawnPoolObjectOnDeathBuffer()
                        {SpawnObjectName = SpawnPoolObjectsOnDeath[i].ObjectName, SpawnChance = SpawnPoolObjectsOnDeath[i].SpawnChance});
                }
            }
        }
        
        if (SpawnAudioObjectsOnDeath != null)
        {
            if (SpawnAudioObjectsOnDeath.Length > 0)
            {
                var spawnAfterDeathBuffer = dstManager.AddBuffer<SpawnSoundObjectOnDeathBuffer>(entity);
                for (int i = 0; i < SpawnAudioObjectsOnDeath.Length; i++)
                {
                    spawnAfterDeathBuffer.Add(new SpawnSoundObjectOnDeathBuffer()
                    {
                        ClipName = SpawnAudioObjectsOnDeath[i].ClipName,
                        Type = SpawnAudioObjectsOnDeath[i].Type,
                        Delay = SpawnAudioObjectsOnDeath[i].Delay,
                        Pitch = SpawnAudioObjectsOnDeath[i].Pitch,
                        PitchTime = SpawnAudioObjectsOnDeath[i].PitchTime,
                    });
                }
            }
        }

        ConvertAncestors(entity, dstManager, conversionSystem);
    }

    public virtual void ConvertAncestors(Entity entity, EntityManager dstManager,
        GameObjectConversionSystem conversionSystem)
    {
        
    }
}
