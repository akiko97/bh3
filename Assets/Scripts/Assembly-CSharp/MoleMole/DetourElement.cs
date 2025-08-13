using UnityEngine;

namespace MoleMole
{
	public class DetourElement
	{
		public uint id;

		public Vector3 targetPosition;

		public float disReachCornerThreshold;

		public Vector3[] corners;

		public bool isCompletePath;

		public uint targetCornerIndex;

		public float lastGetPathTime;
	}
}
