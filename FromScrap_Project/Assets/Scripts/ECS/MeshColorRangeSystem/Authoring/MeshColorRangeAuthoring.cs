using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshColorRangeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private float _hueBaseValue = 0;
    [SerializeField] private float _hueRange = 0.1f;
    [SerializeField] private float _saturationBaseValue = 1;
    [SerializeField] float _saturationRange = 0.1f;
    [SerializeField] private float2 _randomRange = new float2(0,1);
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new MaterialPropertyHUE() {Value = _hueBaseValue + Random.Range(-_hueRange, _hueRange)});
        dstManager.AddComponentData(entity, 
            new MaterialPropertySaturation(){Value = _saturationBaseValue + Random.Range(-_saturationRange, _saturationRange)});
        dstManager.AddComponentData(entity, 
            new MaterialPropertyRandom(){Value = Random.Range(_randomRange.x, _randomRange.y)});
    }
}