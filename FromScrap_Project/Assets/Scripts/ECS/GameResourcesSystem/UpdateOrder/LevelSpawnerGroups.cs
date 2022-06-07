using Unity.Entities;

namespace ECS.GameResourcesSystem.UpdateOrder
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class LevelSpawnerGroup : ComponentSystemGroup { }
}
