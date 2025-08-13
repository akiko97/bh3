using UnityEngine;

namespace MoleMole
{
	public class MonoSpawnPoint : MonoBehaviour
	{
		[SerializeField]
		private bool _showGizmos;

		[SerializeField]
		private bool _wireMode;

		[SerializeField]
		private Color _color = Color.white;

		[SerializeField]
		private Vector3 _gizmosSize;

		[SerializeField]
		private Vector3 _gizmosOffset;

		public Vector3 XZPosition
		{
			get
			{
				return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
			}
		}

		private void OnDrawGizmos()
		{
			if (_showGizmos)
			{
				Color color = Gizmos.color;
				Gizmos.color = _color;
				if (_wireMode)
				{
					Gizmos.DrawWireCube(base.transform.position + _gizmosOffset, _gizmosSize);
				}
				else
				{
					Gizmos.DrawCube(base.transform.position + _gizmosOffset, _gizmosSize);
				}
				Gizmos.color = color;
			}
		}
	}
}
