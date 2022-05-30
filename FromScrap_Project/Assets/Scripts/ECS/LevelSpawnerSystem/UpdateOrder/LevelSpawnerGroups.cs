using Unity.Entities;

namespace ECS.LevelSpawnerSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class LevelSpawnerGroup : ComponentSystemGroup { }
}
