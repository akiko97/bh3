using System;
using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class RelativeToUICamera : EffectCreationOp
	{
		[NonSerialized]
		[InspectorDisabled]
		public string type = "Relateve to UI Camera";

		public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
		{
			MonoInLevelUICamera inLevelUICamera = Singleton<CameraManager>.Instance.GetInLevelUICamera();
			initPos = inLevelUICamera.transform.position;
			initDir = inLevelUICamera.transform.forward;
		}
	}
}
