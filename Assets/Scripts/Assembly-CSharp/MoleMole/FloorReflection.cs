using UnityEngine;

namespace MoleMole
{
	[DisallowMultipleComponent]
	public class FloorReflection : ReflectionBase
	{
		public float floorHeight;

		protected override void Awake()
		{
			base.Awake();
			_reflectionPlanePosition = Vector3.up * floorHeight;
		}

		protected override void Update()
		{
			_reflectionPlanePosition = Vector3.up * floorHeight;
		}
	}
}
