using UnityEngine;

namespace MoleMole
{
	public class MonoTreeBillboard : MonoBehaviour
	{
		public int UpdateRate = 1;

		public float ZOffsetStep = 0.001f;

		public bool CheckVisible;

		private Camera _camera;

		private int _frame = -1;

		private bool _visible;

		private Vector3[] _originalLocalPositions;

		private void Start()
		{
			_camera = Camera.main;
			_originalLocalPositions = new Vector3[base.transform.childCount];
			for (int i = 0; i < base.transform.childCount; i++)
			{
				_originalLocalPositions[i] = base.transform.GetChild(i).localPosition;
			}
		}

		private void Update()
		{
			_frame++;
			if ((!CheckVisible || _visible) && _frame % UpdateRate == 0)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					Transform child = base.transform.GetChild(i);
					Vector3 vector = _camera.transform.position - child.position;
					vector.y = 0f;
					vector.Normalize();
					child.LookAt(child.position + vector);
					child.localPosition = _originalLocalPositions[i] - vector * ZOffsetStep * i;
				}
			}
		}

		private void OnBecameVisible()
		{
			_visible = true;
			_frame = -1;
		}

		private void OnBecameInvisible()
		{
			_visible = false;
		}
	}
}
