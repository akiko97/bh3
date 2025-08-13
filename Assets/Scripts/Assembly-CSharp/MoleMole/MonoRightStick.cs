using UnityEngine;

namespace MoleMole
{
	public class MonoRightStick : MonoBehaviour
	{
		private const float ROTATION_DELTA_X = 5f;

		private const float ROTATION_DELTA_Y = 3f;

		public bool isRotating;

		private void Start()
		{
			isRotating = false;
		}
	}
}
