using Unity.Entities;

namespace LevelingSystem.Components
{
	[GenerateAuthoringComponent]
	public struct LevelComponent : IComponentData
	{
		public int Experience;
		public int Level;
	}
}
