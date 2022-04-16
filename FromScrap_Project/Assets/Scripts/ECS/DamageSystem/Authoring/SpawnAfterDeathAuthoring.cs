using DamageSystem.Components;
using Unity.Entities;
using UnityEngine;

public class SpawnAfterDeathAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Death Info")]
    [SerializeField] private string[] SpawnEntitiesOnDeath;
    [SerializeField] private string[] SpawnPoolObjectsOnDeath;
    
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
                        {SpawnEntity = SpawnEntitiesOnDeath[i]});
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
                        {SpawnObjectName = SpawnPoolObjectsOnDeath[i]});
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
