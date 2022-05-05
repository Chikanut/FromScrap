using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct RandomIndex : IComponentData
{
    public int Value;
}

public partial class MeshColoRangeInitSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    private EntityQuery _querry;
    RandomIndex _randomIndex;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        if (!HasSingleton<RandomIndex>())
        {
            World.EntityManager.CreateEntity(ComponentType.ReadOnly<RandomIndex>());
            SetSingleton(new RandomIndex() {Value = 1});
        }

        _querry = GetEntityQuery(
            ComponentType.ReadOnly<MeshColorRangeComponent>()
        );

        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var randomIndex = GetSingleton<RandomIndex>().Value;
        SetSingleton(new RandomIndex() {Value = randomIndex + _querry.CalculateEntityCount()});
        
        Dependency = Entities.ForEach((Entity entity,
            ref MaterialPropertyHUE hue,
            ref MaterialPropertyRandom random,
            ref MaterialPropertySaturation saturation,
            in MeshColorRangeComponent meshColorRangeComponent) =>
        {
            var r = Random.CreateFromIndex((uint)randomIndex);
            
            var s = meshColorRangeComponent.SaturationBaseValue + r.NextFloat(-meshColorRangeComponent.SaturationRange,
                meshColorRangeComponent.SaturationRange);
            saturation.Value = s;
            
            var h = meshColorRangeComponent.HueBaseValue + r.NextFloat(-meshColorRangeComponent.HueRange,
                meshColorRangeComponent.HueRange);
            
            hue.Value = h;
            
            random.Value = r.NextFloat(meshColorRangeComponent.RandomRange.x,meshColorRangeComponent.RandomRange.y);

            ecb.RemoveComponent<MeshColorRangeComponent>(entity);
        }).Schedule(Dependency);
        
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}