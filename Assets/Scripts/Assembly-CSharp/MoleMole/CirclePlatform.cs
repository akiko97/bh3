using UnityEngine;

namespace MoleMole
{
	public sealed class CirclePlatform : BasePlatform
	{
		public uint Radius { get; private set; }

		public CirclePlatform(MonoBasePerpStage stageOwner, uint radius)
			: base(stageOwner)
		{
			Radius = radius;
		}

		public override Vector3 GetARandomPlace()
		{
			Vector2 vector = Random.insideUnitCircle * Radius;
			return new Vector3(vector.x, 0f, vector.y);
		}
	}
}
