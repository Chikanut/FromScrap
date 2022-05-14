using Unity.Entities;

namespace ECS.DynamicTerrainSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class DynamicTerrainSimulationGroup: ComponentSystemGroup{}
    
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DynamicTerrainVisualizationGroup: ComponentSystemGroup{}
}
