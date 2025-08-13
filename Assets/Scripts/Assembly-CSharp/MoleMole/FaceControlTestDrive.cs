using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class FaceControlTestDrive : MonoBehaviour
	{
		public MonoAvatarFaceControl faceControl;

		public Transform positionEdit;

		public Transform lookAtEdit;

		public Transform positionNormal;

		public Transform lookAtNormal;

		public Transform cameraTransform;

		private int _currentFaceIndex;

		private void OnGUI()
		{
		}
	}
}
