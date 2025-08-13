using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class RelativeToTransformPath : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "Relative To Transform Path";

		public string TransformPath;

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			Transform transform = entity.transform.Find(TransformPath);
			initPos = transform.position;
		}
	}
}
