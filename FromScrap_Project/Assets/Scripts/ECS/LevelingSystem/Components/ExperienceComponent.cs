using Unity.Entities;

namespace LevelingSystem.Components
{
	[GenerateAuthoringComponent]
	public struct ExperienceComponent : IComponentData
	{
		public int Value;
		public bool Gathered;
	}
}
