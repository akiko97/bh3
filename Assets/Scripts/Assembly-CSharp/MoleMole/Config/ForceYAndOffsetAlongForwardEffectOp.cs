using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class ForceYAndOffsetAlongForwardEffectOp : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "ForceY And Offset Along Forward Effect";

		public float y;

		public float offset;

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			initPos.y = y * initScale.y;
			initPos += Vector3.Scale(initDir, initScale) * offset;
		}
	}
}
