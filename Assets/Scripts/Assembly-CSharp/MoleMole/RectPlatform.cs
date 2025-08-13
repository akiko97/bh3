using UnityEngine;

namespace MoleMole
{
	public sealed class RectPlatform : BasePlatform
	{
		public uint Width { get; private set; }

		public uint Height { get; private set; }

		public RectPlatform(MonoBasePerpStage stageOwner, uint width, uint height)
			: base(stageOwner)
		{
			Width = width;
			Height = height;
		}

		public override Vector3 GetARandomPlace()
		{
			return new Vector3((Random.value - 0.5f) * (float)Width, 0f, (Random.value - 0.5f) * (float)Height);
		}
	}
}
