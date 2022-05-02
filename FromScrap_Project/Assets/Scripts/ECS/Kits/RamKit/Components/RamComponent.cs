using Unity.Entities;
using Unity.Mathematics;

namespace Ram.Components
{
	public struct RamComponent : IComponentData
	{
		public float2 DamageRange;
		public float2 ImpulseRange;
		public float2 SpeedRange;
	}
}
