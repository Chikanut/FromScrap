using Unity.Entities;

namespace LevelingSystem.Components
{
	public struct ExperienceComponent : IComponentData
	{
		public int Value;
		public bool Gathered;
	}
}
