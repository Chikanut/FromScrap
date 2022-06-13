using System;
using Unity.Entities;


namespace ForceField.Components
{
	[GenerateAuthoringComponent]
	[Serializable]
	public struct ForceFieldComponent : IComponentData
	{
		public float LifeTime;
		public float Radius;
		public float Force;
		public bool ForceIn;

		public float CurrentLifeTime;
	}
}
