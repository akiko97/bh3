using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class RelativeToAttachPoint : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "Relative To Attach Point";

		public string AttachPoint;

		public Vector3 OffsetVec3;

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			Transform attachPoint = entity.GetComponent<BaseMonoEntity>().GetAttachPoint(AttachPoint);
			initPos = attachPoint.position;
			float angle = Vector3.Angle(attachPoint.forward, Vector3.forward);
			initPos += Quaternion.AngleAxis(angle, Vector3.up) * Vector3.Scale(OffsetVec3, initScale);
		}
	}
}
