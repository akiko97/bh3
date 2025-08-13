using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class RelativeToAttachPointAndForceY : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "Relative To Attach Point and Force Y";

		public string AttachPoint;

		public Vector3 OffsetVec3;

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			Transform attachPoint = entity.GetComponent<BaseMonoEntity>().GetAttachPoint(AttachPoint);
			initPos = attachPoint.position;
			initPos.y = 0f;
			initPos += OffsetVec3;
		}
	}
}
