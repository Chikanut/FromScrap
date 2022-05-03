using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class QuadrantSystem : SystemBase
{
    private const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 50;

    private BeginSimulationEntityCommandBufferSystem _ecbSystem;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        _ecbSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
        
         Dependency = Entities.WithName("QuadrantEntityMapping").ForEach((Entity entity, int entityInQueryIndex, in QuadrantEntityData quadrantEntityData, in LocalToWorld translation) =>
            {
                var hashMapKey = (int) (math.floor(translation.Position.x / quadrantCellSize) +
                                        (quadrantYMultiplier * math.floor(translation.Position.z / quadrantCellSize)));

                ecb.SetComponent(entityInQueryIndex, entity,
                    new QuadrantEntityData {HashKey = hashMapKey, Type = quadrantEntityData.Type});

            }).ScheduleParallel(Dependency);
         
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}

