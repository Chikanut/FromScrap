using Unity.Entities;


namespace MoveTo.Components
{
	public struct MoveToComponent : IComponentData
	{
		public Entity Target;
		public float Speed;
	}
}
