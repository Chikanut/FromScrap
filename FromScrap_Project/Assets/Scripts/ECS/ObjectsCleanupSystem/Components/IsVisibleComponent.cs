using Unity.Entities;

namespace IsVisible.Components
{
	public struct IsVisibleComponent : IComponentData
	{
		public bool Value;
		public float ObjectRadius;
	}
}
