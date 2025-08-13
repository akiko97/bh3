using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class RotateAroundYEffectOp : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "RotateAroundYEffectOp";

		public float rotation = 180f;

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			initDir = Quaternion.AngleAxis(rotation, Vector3.up) * initDir;
		}
	}
}
