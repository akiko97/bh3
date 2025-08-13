using UnityEngine;

namespace MoleMole
{
	public class MonoRotateY : MonoBehaviour
	{
		public float RotateYAnglePerSecond;

		public float TotalAngle;

		private float _angleRotated;

		[Header("Rotate Target")]
		public Transform targetTransform;

		[Header("Loop")]
		public bool Loop;

		public void Awake()
		{
			if (targetTransform == null)
			{
				targetTransform = base.transform;
			}
			_angleRotated = 0f;
		}

		public void Update()
		{
			Vector3 localEulerAngles = targetTransform.localEulerAngles;
			float num = RotateYAnglePerSecond * Time.deltaTime;
			localEulerAngles.y += num;
			targetTransform.localEulerAngles = localEulerAngles;
			_angleRotated += num;
		}
	}
}
