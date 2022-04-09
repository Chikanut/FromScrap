using Unity.Entities;

namespace Lifetime.Components
{
	[GenerateAuthoringComponent]
	public struct LifetimeComponent : IComponentData
	{
		public float CurrentLifetime;
		public float MaxLifeTime;
	}
}
