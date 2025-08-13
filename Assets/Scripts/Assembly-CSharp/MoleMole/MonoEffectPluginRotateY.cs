using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginRotateY : BaseMonoEffectPlugin
	{
		public float RotateYAnglePerSecond;

		public float TotalAngle;

		private float _angleRotated;

		[Header("Rotate Target")]
		public Transform targetTransform;

		[Header("Loop")]
		public bool Loop;

		protected override void Awake()
		{
			base.Awake();
			if (targetTransform == null)
			{
				targetTransform = base.transform;
			}
			_angleRotated = 0f;
		}

		public override void Setup()
		{
			_angleRotated = 0f;
		}

		public void Update()
		{
			Vector3 localEulerAngles = targetTransform.localEulerAngles;
			float num = RotateYAnglePerSecond * Time.deltaTime * _effect.TimeScale;
			localEulerAngles.y += num;
			targetTransform.localEulerAngles = localEulerAngles;
			_angleRotated += num;
		}

		public override bool IsToBeRemove()
		{
			return !Loop && _angleRotated > TotalAngle;
		}

		public override void SetDestroy()
		{
			_angleRotated = TotalAngle - RotateYAnglePerSecond * 0.3f;
		}
	}
}
