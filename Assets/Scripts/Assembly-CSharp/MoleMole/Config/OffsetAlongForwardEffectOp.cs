using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class OffsetAlongForwardEffectOp : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "Offset Along Forward Effect";

		public float offset;

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			initPos += Vector3.Scale(initDir, initScale) * offset;
		}
	}
}
