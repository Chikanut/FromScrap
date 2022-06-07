using Unity.Entities;

namespace Collectables.Components
{
	[GenerateAuthoringComponent]
	public struct CollectableTriggerComponent : IComponentData
	{
		public Entity MainObject;
	}
}
